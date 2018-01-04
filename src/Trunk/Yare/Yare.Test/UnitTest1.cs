/// <copyright>
/// Copyright (c) 2013, Vargas Isaias Patag
/// All rights reserved.
/// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
/// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
/// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
/// </copyright>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using Yare.Lib.Rules;
using Yare.Lib;
using Yare.Lib.Enums;
using System.IO;
using ProtoBuf;

namespace Yare.Test
{
    public class Pricing
    {
        public decimal Cost { get; set; }
        public decimal SomeOtherCost { get; set; }
        public string Name { get; set; }
    }

    public class NumbericObj
    {
        public decimal DecimalValue { get; set; }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RuleWithinARuleWithDemoOfProtobufNet()
        {
            var rule = new MDASRule("2*Cost", MDASOperationEnum.Add, "5");
            var rule1 = new MDASRule(rule, MDASOperationEnum.Multiply, "5");
            var rule2 = new EqualityRule(rule1, EqualityOperationEnum.GreaterThan, "SomeOtherCost/1");
            var rule3 = new EqualityRule("Cost", EqualityOperationEnum.GreaterThan, "20000");
            var rule4 = new BitWiseRule(rule2, BitWiseOperationEnum.And, rule3);
            var rule5 = new EqualityRule("Name", EqualityOperationEnum.Equal, "Aia");
            var rule6 = new BitWiseRule(rule4, BitWiseOperationEnum.And, rule5);

            RuleBase protobuffedRule = null;
            using (var mem = new MemoryStream())
            {
                Serializer.Serialize(mem, rule6);
                mem.Position = 0;
                protobuffedRule = Serializer.Deserialize<RuleBase>(mem);
            }

            string ruleText;
            Func<Pricing, bool> compiledRule = protobuffedRule.CompileRule<Pricing>(out ruleText);
            Pricing p = new Pricing()
            {
                Cost = 100001,
                SomeOtherCost = 5,
                Name = "Aia"
            };

            var result = compiledRule(p);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ArithmeticConditionalRule()
        {
            Pricing p = new Pricing()
            {
                SomeOtherCost = 5
            };

            var r = new EqualityRule("1+2*3+SomeOtherCost*1", EqualityOperationEnum.GreaterThan, "11");
            string ruleText;
            var compiledRule = r.CompileRule<Pricing>(out ruleText);
            var result = compiledRule(p);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluatorDecimalConstant()
        {
            var compiledRule = RuleBase.CompileDecimalEvaluator("5-1*3+1/2");
            var result = compiledRule();

            decimal expected = 2.5M;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EvaluatorDecimalWithParams()
        {
            Pricing p = new Pricing()
            {
                Cost = 100
            };
            var compiledRule = RuleBase.CompileDecimalEvaluatorWithParameter<Pricing>("5-1*3+1/2+Cost");
            var result = compiledRule(p);

            decimal expected = 102.5M;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SingleRuleItemNumberic()
        {
            Assert.IsTrue(testSingleRuleItemNumberic(new Decimal(2.0), "<=", new Decimal(2.01)));
            Assert.IsTrue(testSingleRuleItemNumberic(new Decimal(2.0), "<", new Decimal(2.01)));

            Assert.IsTrue(testSingleRuleItemNumberic(3, ">", 2));
            Assert.IsTrue(testSingleRuleItemNumberic(2, ">", 0));
            Assert.IsTrue(testSingleRuleItemNumberic(2, "<", 3));
            Assert.IsTrue(testSingleRuleItemNumberic(2, ">=", 2));
            Assert.IsTrue(testSingleRuleItemNumberic(2, "<=", 2));
            Assert.IsTrue(testSingleRuleItemNumberic(2, "=", 2));

            Assert.IsTrue(testSingleRuleItemNumberic(0, ">", -1));
            Assert.IsTrue(testSingleRuleItemNumberic(-1, ">=", -1));
            Assert.IsTrue(testSingleRuleItemNumberic(-1, "<=", 0));
            Assert.IsTrue(testSingleRuleItemNumberic(-1, ">=", -2));
            Assert.IsTrue(testSingleRuleItemNumberic(-1, ">", -2));

            Assert.IsTrue(testSingleRuleItemNumberic(new Decimal(-10), ">", new Decimal(-10.2)));
            Assert.IsTrue(testSingleRuleItemNumberic(new Decimal(-10), ">=", new Decimal(-10.2)));
            Assert.IsTrue(testSingleRuleItemNumberic(new Decimal(-10.3), "<", new Decimal(-10.2)));
        }

        private bool testSingleRuleItemNumberic(decimal left, string op, decimal right)
        {

            EqualityRule rule;
            if (right >= 0)
                rule = new EqualityRule("DecimalValue", getEqualityOperation(op), right.ToString());
            else
            {
                string number = "0-" + Decimal.Negate(right).ToString();
                Console.WriteLine("right: {0}, numberString: {1}", right, number);
                rule = new EqualityRule("DecimalValue", getEqualityOperation(op), number);
            }


            RuleBase protobuffedRule = null;
            using (var mem = new MemoryStream())
            {
                Serializer.Serialize(mem, rule);
                mem.Position = 0;
                protobuffedRule = Serializer.Deserialize<RuleBase>(mem);
            }

            string ruleText;
            Func<NumbericObj, bool> compiledRule = protobuffedRule.CompileRule<NumbericObj>(out ruleText);
            NumbericObj p = new NumbericObj()
            {
                DecimalValue = left
            };

            return compiledRule(p);
        }

        private EqualityOperationEnum getEqualityOperation(string op)
        {
            switch (op)
            {
                case "=":
                    return EqualityOperationEnum.Equal;
                case ">":
                    return EqualityOperationEnum.GreaterThan;
                case ">=":
                    return EqualityOperationEnum.GreaterThanOrEqual;
                case "<":
                    return EqualityOperationEnum.LessThan;
                case "<=":
                    return EqualityOperationEnum.LessThanOrEqual;
                case "!=":
                    return EqualityOperationEnum.NotEqual;
                default:
                    throw new Exception("Unsupported equality operation!");
            }
        }
    }
}

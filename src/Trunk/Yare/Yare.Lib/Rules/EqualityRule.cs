﻿/// <copyright>
/// Copyright (c) 2013, Vargas Isaias Patag
/// All rights reserved.
/// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
/// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
/// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
/// </copyright>
using Yare.Lib.Enums;
using Yare.Lib.Utilities;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yare.Lib.Rules
{
    [ProtoContract]
    [Serializable]
    public sealed class EqualityRule : RuleBase
    {
        private EqualityRule()
            : base()
        { }

        public EqualityRule(string left, EqualityOperationEnum op, string right)
            : base(left, OperationEnumUtility.ConvertToString(op), right)
        {
        }
        public EqualityRule(RuleBase primaryRule, EqualityOperationEnum op, string right)
            : base(primaryRule, OperationEnumUtility.ConvertToString(op), right)
        {
        }
        public EqualityRule(RuleBase primaryRule, EqualityOperationEnum op, RuleBase secondaryRule)
            : base(primaryRule, OperationEnumUtility.ConvertToString(op), secondaryRule)
        {
        }
    }
}

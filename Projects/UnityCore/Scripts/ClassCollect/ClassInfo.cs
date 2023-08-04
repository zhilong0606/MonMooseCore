using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonMoose.Core.ClassCollect
{
    public class ClassInfo
    {
        public Type type;
        public List<ClassMemberInfo> memberList = new List<ClassMemberInfo>();
    }
}

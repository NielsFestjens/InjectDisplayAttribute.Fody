using System;
using InjectDisplayAttribute.ExampleTarget;
using InjectDisplayAttribute.ExampleTarget.Resources.Properties;

[assembly: InjectDisplayAttribute(TypeMatchPattern = "^InjectDisplayAttribute.ExampleTarget.*$", ResourceType = typeof(Resources))]

namespace InjectDisplayAttribute.ExampleTarget
{
    public class InjectDisplayAttributeAttribute : Attribute
    {
        public string TypeMatchPattern { get; set; }
        public Type ResourceType { get; set; }
    }
}
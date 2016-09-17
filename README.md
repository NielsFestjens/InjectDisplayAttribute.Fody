# InjectDisplayAttribute.Fody
A Fody add-in to inject the MVC Display Attribute.

Checkout InjectDisplayAttribute.ExampleTarget for usage.

Add an InjectDisplayAttributeAttribute like this:

	public class InjectDisplayAttributeAttribute : Attribute
    {
        public string TypeMatchPattern { get; set; }
        public Type ResourceType { get; set; }
    }

Then reference and configure it like this:

	[assembly: InjectDisplayAttribute(TypeMatchPattern = "^InjectDisplayAttribute.ExampleTarget.*$", ResourceType = typeof(Resources))]

TypeMatchPattern is a regex pattern, all classes with FullName matching the regex will be processed.
Nested classes will be processed.
ResourceType is the type of the resource file to use.
Properties that already have the Display attribute defined will not be changed.
If the resource file does not contain an item with the name of the property, no Display attribute will be defined.
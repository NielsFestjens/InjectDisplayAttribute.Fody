using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using InjectDisplayAttribute.Fody;
using Mono.Cecil;

public class ModuleWeaver
{
    private TypeReference _typeTypeDefinition;
    private MethodReference _attributeConstructor;

    public ModuleDefinition ModuleDefinition { get; set; }
    public string SolutionDirectoryPath { get; set; }

    public void Execute()
    {
        _typeTypeDefinition = ModuleDefinition.ImportReference(typeof(Type));
        _attributeConstructor = ModuleDefinition.GetConstructor("System.ComponentModel.DataAnnotations", "DisplayAttribute");

        var assemblyAttributes = ModuleDefinition.Assembly.CustomAttributes.Where(x => x.AttributeType.Name == "InjectDisplayAttributeAttribute");
        foreach (var assemblyAttribute in assemblyAttributes)
        {
            var typeMatchPattern = assemblyAttribute.GetPropertyValue<string>("TypeMatchPattern");
            var resourceType = assemblyAttribute.GetPropertyValue<TypeReference>("ResourceType");
            AddDisplayAttribute(typeMatchPattern, resourceType);
        }
    }

    private void AddDisplayAttribute(string typeMatchPattern, TypeReference resourceType)
    {
        var resource = resourceType.ResolveFromDirectory(Path.GetDirectoryName(ModuleDefinition.FullyQualifiedName), SolutionDirectoryPath);

        foreach (var type in ModuleDefinition.Types)
        {
            AddDisplayAtributeToType(type, resourceType, resource, typeMatchPattern);
        }
    }

    private void AddDisplayAtributeToType(TypeDefinition type, TypeReference resourceType, TypeDefinition resource, string typeMatchPattern)
    {
        if (Regex.IsMatch(type.FullName, typeMatchPattern))
        {
            foreach (var property in type.Properties.Where(property => !property.HasAttribute("System.ComponentModel.DataAnnotations.DisplayAttribute")))
            {
                if (!resource.Properties.Any(x => x.Name == property.Name))
                    continue;

                var diplayAttribute = new CustomAttribute(_attributeConstructor);
                diplayAttribute.AddProperty("Name", ModuleDefinition.TypeSystem.String, property.Name);
                diplayAttribute.AddProperty("ResourceType", _typeTypeDefinition, resourceType);
                property.CustomAttributes.Add(diplayAttribute);
            }
        }

        foreach (var nestedType in type.NestedTypes)
        {
            AddDisplayAtributeToType(nestedType, resourceType, resource, typeMatchPattern);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using InjectDisplayAttribute.Fody;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class WeaverTests
{
    private Assembly _assembly;
    private string _assemblyPath;

    [TestFixtureSetUp]
    public void Setup()
    {
        var projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\InjectDisplayAttribute.ExampleTarget\InjectDisplayAttribute.ExampleTarget.csproj"));
        _assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\InjectDisplayAttribute.ExampleTarget.dll");
#if (!DEBUG)
        _assemblyPath = _assemblyPath.Replace("Debug", "Release");
#endif
        
        var moduleDefinition = ModuleDefinition.ReadModule(_assemblyPath);
        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition
        };

        weavingTask.Execute();
        moduleDefinition.Write(_assemblyPath);

        _assembly = Assembly.LoadFile(_assemblyPath);
        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.AssemblyResolve += (sender, args) =>
        {
            var assemblyPath = Path.Combine(Path.GetDirectoryName(_assemblyPath), new AssemblyName(args.Name).Name + ".dll");
            return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
        };
    }

    [Test]
    public void ValidateAttributeIsInjected()
    {
        var model = _assembly.GetType("InjectDisplayAttribute.ExampleTarget.TargetModel");
        var propertyToGetAnAttribute = model.GetProperty("PropertyToGetAnAttribute");
        var propertyToGetAnAttributeAttribute = propertyToGetAnAttribute.GetCustomAttributes(typeof (DisplayAttribute), false).Single() as DisplayAttribute;
        Assert.AreEqual(propertyToGetAnAttributeAttribute.Name, "PropertyToGetAnAttribute");
        Assert.AreEqual(propertyToGetAnAttributeAttribute.ResourceType.FullName, "InjectDisplayAttribute.ExampleTarget.Resources.Properties.Resources");

        var dto = _assembly.GetType("InjectDisplayAttribute.ExampleTarget.TargetModel+TargetDto");
        var dtoPropertyToGetAnAttribute = dto.GetProperty("PropertyToGetAnAttribute");
        var dtoPropertyToGetAnAttributeAttribute = dtoPropertyToGetAnAttribute.GetCustomAttributes(typeof (DisplayAttribute), false).Single() as DisplayAttribute;
        Assert.AreEqual(dtoPropertyToGetAnAttributeAttribute.Name, "PropertyToGetAnAttribute");
        Assert.AreEqual(dtoPropertyToGetAnAttributeAttribute.ResourceType.FullName, "InjectDisplayAttribute.ExampleTarget.Resources.Properties.Resources");

        var propertyThatHasNoTranslation = model.GetProperty("PropertyThatHasNoTranslation");
        var propertyThatHasNoTranslationAttribute = propertyThatHasNoTranslation.GetCustomAttributes(typeof(RequiredAttribute), false).SingleOrDefault();
        Assert.IsNull(propertyThatHasNoTranslationAttribute);

        var propertyToLeaveAsIs = model.GetProperty("PropertyToLeaveAsIs");
        var propertyToLeaveAsIsAttribute = propertyToLeaveAsIs.GetCustomAttributes(typeof(DisplayAttribute), false).Single() as DisplayAttribute;
        Assert.IsNull(propertyToLeaveAsIsAttribute.Name);
        Assert.IsNull(propertyToLeaveAsIsAttribute.ResourceType);
    }
}
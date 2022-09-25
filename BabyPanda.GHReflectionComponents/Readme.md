# GH Reflection Components
**WORK IN PROGRESS**

A library used to generate grasshopper components from C# classes where inputs and outputs are tagged with attributes.

This is achieved in following steps:
1. Create a C# class where properties with the inputs/outputs are tagged with the attributes `Input` & `Output`. The class should also inherit from the class `ComponentTemplate` which has the `SolveFromProperties()` method. This is the method which takes values placed into the input proporties, solves for a solution, and puts the result back into the output properties.
2. Before the class is first used, reflection is used to read from the class and generate data in the ComponentReflectionData. This is cached. This reflection currently happens in ComponentTemplate, with a singleton-like check.
3. When the grasshopper component is created, the ComponentReflectionData is used to create input and output parameters, and to execute the component with reflection to inject data into the input/output properties. **This part is work in progress.**

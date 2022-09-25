This CS project is a clone of `Microsoft.Data.Analysis` as of 15/08/2022, which is under an MIT license.

The repository is at https://github.com/dotnet/machinelearning,
... more specifically https://github.com/dotnet/machinelearning/tree/main/src/Microsoft.Data.Analysis

### Changes

Tweaks from original repo as follows:

- Changed to Framework v4.8. (Previously .NETStandard v2.0)
- Removed references to Apache.Arrow
- Removed references to IDataView

### Motivations

This clone was necessary as Grasshopper cannot run a .NETStandard v2.0 dll within the C# scripting component. Apache.Arrow & IDataView from Microsoft.ML also eliminates any dependancies in .NETStandard.

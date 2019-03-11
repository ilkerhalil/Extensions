///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument ("target", "Default");
var configuration = Argument ("configuration", "Release");
var artifactDirectory = "./artifacts";
var solutions = GetFiles ("./**/*.sln");
var solutionPaths = solutions.Select (solution => solution.GetDirectory ());

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////
Task ("Clean")
   .Does (() => {
      var cleanDirectories = new [] {
      artifactDirectory

      };

      foreach (var path in solutionPaths) {
         Information ("Cleaning {0}", path);
         CleanDirectories (path + "/**/bin/" + configuration);
         CleanDirectories (path + "/**/obj/" + configuration);
      }

      CleanDirectories (cleanDirectories);
      DotNetCoreClean (".");
   });

Task ("Restore")
   .Does (() => {
      foreach (var path in solutionPaths) {
         DotNetCoreRestore (path.FullPath);
      }
   });

Task ("Build")
   .IsDependentOn ("Clean")
   .IsDependentOn ("Restore")
   .Does (() => {
      foreach (var path in solutionPaths) {
         DotNetCoreBuild (path.FullPath);
      }
   });
Task("Pack")
   .Does(() =>
{
   DotNetCorePack(
        "./src/ExtensionMethods",
        new DotNetCorePackSettings
        {
            OutputDirectory = packageOutputDirectory,
            Configuration = configuration
        });
});

RunTarget (target);
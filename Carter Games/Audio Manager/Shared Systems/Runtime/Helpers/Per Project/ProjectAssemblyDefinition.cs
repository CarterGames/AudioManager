using System.Reflection;

namespace CarterGames.Assets.Shared.PerProject
{
    public static class ProjectAssemblyDefinition
    {
        public static Assembly[] ProjectEditorAssemblies
        {
            get
            {
                return new Assembly[]
                {
                    Assembly.Load("CarterGames.AudioManager.Editor"),
                    Assembly.Load("CarterGames.AudioManager.Runtime"),
                    Assembly.Load("CarterGames.Shared.Editor.AudioManager"),
                    Assembly.Load("CarterGames.Shared.AudioManager")
                };
            }
        }
        
        
        public static Assembly[] ProjectRuntimeAssemblies
        {
            get
            {
                return new Assembly[]
                {
                    Assembly.Load("CarterGames.AudioManager.Runtime"),
                    Assembly.Load("CarterGames.Shared.AudioManager")
                };
            }
        }
    }
}
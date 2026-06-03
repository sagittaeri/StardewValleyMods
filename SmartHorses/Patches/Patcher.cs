namespace SmartHorses
{
    using HarmonyLib;
    using System;
    public class Patcher
    {
        private static SmartHorses mod;

        public static void PatchAll(SmartHorses smartHorses)
        {
            mod = smartHorses;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                InteractPatches.ApplyPatches(smartHorses, harmony);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to setup required patches\n{0}:", e.ToString());
            }
        }
    }
}

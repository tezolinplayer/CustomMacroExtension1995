using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        public override void Init()
        {
            // Título principal
            MainGate.Text = "Daniel Mod PS5";

            // Trocamos 'CreateNVN' por 'CreateTVN' porque o seu sistema só aceita este
            MainGate.Add(CreateTVN("Ativar Anti-Recoil"));
            MainGate.Add(CreateTVN("Ativar Aim Color"));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            // Trocamos '.Value' por '.Enable' para sumir o erro CS1061
            if (MainGate[0].Enable) 
            {
                // Lógica de Recoil aqui
            }

            if (MainGate[1].Enable) 
            {
                // Lógica de Aim Color aqui
            }
        }
    }
}

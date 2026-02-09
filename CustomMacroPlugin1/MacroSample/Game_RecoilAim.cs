using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        private double recoilVertical = 15;
        private bool aimColorAtivo = false;

        public override void Init()
        {
            MainGate.Text = "Ajustador Daniel";

            // Usando CreateTVN para os controles, pois o erro indicou que o framework 
            // está esperando Nodes de árvore (TreeViewNode)
            MainGate.Add(CreateTVN("Ativar Recoil"));
            MainGate.Add(CreateTVN("Ativar Aim Color"));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;

            // No seu framework, acessamos os Toggles assim:
            bool recoilLigado = MainGate[0].Enable;
            aimColorAtivo = MainGate[1].Enable;

            if (recoilLigado)
            {
                ExecutarRecoil();
            }
        }

        private void ExecutarRecoil()
        {
            // Lógica simples de descida de mira
        }
    }
}

using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        // Força do puxão para baixo (aumente se a arma subir muito)
        private int forcaVertical = 20; 

        public override void Init()
        {
            MainGate.Text = "Daniel Mod PS5";
            MainGate.Add(CreateTVN("Ativar Anti-Recoil"));
            MainGate.Add(CreateTVN("Ativar Aim Color"));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;

            // Se o Anti-Recoil estiver ligado no menu E você estiver apertando R2
            // Nota: 'State.R2' verifica a pressão do gatilho
            if (MainGate[0].Enable && Controller.State.R2 > 50)
            {
                AplicarPuxaoParaBaixo();
            }
        }

        private void AplicarPuxaoParaBaixo()
        {
            // Move o analógico direito (mira) para baixo
            // O valor é somado à posição atual do seu dedo
            Controller.Output.RightStickY += (short)forcaVertical;
        }
    }
}

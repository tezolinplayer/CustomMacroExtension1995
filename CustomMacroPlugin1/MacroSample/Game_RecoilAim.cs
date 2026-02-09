using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using System.Drawing;

namespace CustomMacroPlugin1.MacroSample
{
    // O SortIndex define a posição na lista do DS4Windows
    [SortIndex(205)] 
    partial class Game_RecoilAim : MacroBase
    {
        // Variáveis para guardar os valores dos sliders
        private double recoilVertical;
        private double recoilHorizontal;
        private string aimColorTarget = "#FF0000"; // Padrão: Vermelho

        public override void Init()
        {
            // Título principal que aparece no menu
            MainGate.Text = "Configuração de Recoil & Aim";

            // 1. Slider para Recoil Vertical (Nome, Valor Inicial, Min, Max, Passo)
            MainGate.Add(CreateNVN("Recoil Vertical", 15, 0, 100, 1));

            // 2. Slider para Recoil Horizontal
            MainGate.Add(CreateNVN("Recoil Horizontal", 0, -50, 50, 1));

            // 3. Toggle para Ativar o Aim Color
            MainGate.Add(CreateTVN("Ativar Detecção de Cor"));
        }

        public override void UpdateState()
        {
            // Se o botão principal estiver desligado, não faz nada
            if (MainGate.Enable is false) { return; }

            // Lendo os valores dos Sliders
            recoilVertical = MainGate[0].Value;
            recoilHorizontal = MainGate[1].Value;

            // Lógica de execução do Recoil
            ExecutarRecoil();

            // Lógica do Aim Color (Ativa apenas se o Toggle [2] estiver ligado)
            if (MainGate[2].Enable)
            {
                ProcessarAimColor();
            }
        }

        private void ExecutarRecoil()
        {
            // Aqui você deve usar o comando de movimentação do seu framework
            // Exemplo genérico: mover o analógico baseado nos sliders
            // Controller.SetRightStick(recoilHorizontal, recoilVertical);
        }

        private void ProcessarAimColor()
        {
            // Espaço reservado para sua lógica de detecção de pixels
            // Você pode usar o valor 'aimColorTarget' para comparar cores na tela
        }
    }
}

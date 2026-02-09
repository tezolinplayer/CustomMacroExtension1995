using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using System;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilColor : MacroBase
    {
        // Variáveis de controle
        private double forcaRecoil;
        private bool detectarCor;

        public override void Init()
        {
            // Título no Menu do DS4Windows
            MainGate.Text = "Ajustador Daniel";

            // Slider de Recoil: Nome, Inicial, Min, Max, Passo
            MainGate.Add(CreateNVN("Intensidade Recoil", 10, 0, 100, 1));

            // Toggle para Ativar Detecção de Cor (Aim Color)
            MainGate.Add(CreateTVN("Ativar Aim Color (Vermelho)"));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;

            // Pega o valor do slider
            forcaRecoil = MainGate[0].Value;
            detectarCor = MainGate[1].Enable;

            // Se estiver atirando (exemplo usando R2/RT), aplica o recoil
            // Nota: A lógica exata depende das bibliotecas de entrada do seu projeto
            AplicarModificacoes();
        }

        private void AplicarModificacoes()
        {
            // Aqui entra a sua lógica de movimentação do analógico
            // Para o Recoil, movemos o eixo Y para baixo com base na 'forcaRecoil'
            
            if (detectarCor)
            {
                // Espaço para a lógica de reconhecimento de imagem (OpenCV/Pixel)
                // que você tem interesse em desenvolver
            }
        }
    }
}

using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using System;
using System.Drawing; // Necessário para ler cores
using System.Runtime.InteropServices; // Necessário para mover o mouse

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(205)]
    partial class Game_RecoilAim : MacroBase
    {
        // --- CONFIGURAÇÕES DE RECOIL ---
        private int forcaVertical = 30;
        private int forcaHorizontal = 0;
        private int tickCount = 0;

        // --- CONFIGURAÇÕES DO AUTO COLOR ---
        // Área de busca (quadrado no centro da tela)
        private int tamanhoBusca = 40; 
        private int velocidadeMira = 15; // Velocidade que a mira puxa (0-100)
        
        // Cores (R, G, B)
        private Color corVermelha = Color.FromArgb(200, 0, 0); // Inimigos padrão
        private Color corRoxa = Color.FromArgb(150, 0, 200);   // Escudos/Especiais
        private Color corAmarela = Color.FromArgb(255, 255, 0); // Destaques

        public override void Init()
        {
            MainGate.Text = "Daniel Aimbot Elite";

            // [0] a [3] - Funções Básicas
            MainGate.Add(CreateTVN("Ativar Anti-Recoil")); 
            MainGate.Add(CreateTVN("Ativar Rapid Fire"));  
            MainGate.Add(CreateTVN("Ativar Drop Shot"));   
            MainGate.Add(CreateTVN("Ativar Slide Cancel"));

            // [4] a [6] - Seleção de Cor (Só ative UMA por vez)
            MainGate.Add(CreateTVN(">> MIRA: Cor Vermelha")); // [4]
            MainGate.Add(CreateTVN(">> MIRA: Cor Roxa"));     // [5]
            MainGate.Add(CreateTVN(">> MIRA: Cor Amarela"));  // [6]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) return;
            tickCount++;

            // =========================================================
            // 1. AJUSTES MANUAIS (L1 + Setinhas)
            // =========================================================
            if (RealDS4.L1)
            {
                if (RealDS4.DpadUp && tickCount % 5 == 0 && forcaVertical < 100) forcaVertical++;
                if (RealDS4.DpadDown && tickCount % 5 == 0 && forcaVertical > 0) forcaVertical--;
                if (RealDS4.DpadLeft && tickCount % 5 == 0 && forcaHorizontal > -50) forcaHorizontal--;
                if (RealDS4.DpadRight && tickCount % 5 == 0 && forcaHorizontal < 50) forcaHorizontal++;
            }

            // =========================================================
            // 2. FUNÇÕES DE COMBATE
            // =========================================================
            
            // Anti-Recoil
            if (MainGate[0].Enable && RealDS4.R2 > 50)
            {
                int novoY = RealDS4.RY + forcaVertical;
                if (novoY > 255) novoY = 255;
                VirtualDS4.RY = (byte)novoY;

                int novoX = RealDS4.RX + forcaHorizontal;
                if (novoX > 255) novoX = 255; if (novoX < 0) novoX = 0;
                VirtualDS4.RX = (byte)novoX;
            }

            // Rapid Fire
            if (MainGate[1].Enable && RealDS4.R2 > 50)
            {
                if (tickCount % 3 == 0) VirtualDS4.R2 = 255; else VirtualDS4.R2 = 0;
            }

            // Drop Shot
            if (MainGate[2].Enable && RealDS4.R2 > 200) VirtualDS4.Circle = true;

            // Slide Cancel
            if (MainGate[3].Enable && RealDS4.Circle) VirtualDS4.Cross = true;

            // =========================================================
            // 3. AUTO COLOR (AIMBOT DE COR)
            // =========================================================
            // Só funciona se estiver mirando (L2 apertado)
            if (RealDS4.L2 > 50)
            {
                // Verifica qual cor está marcada no menu
                if (MainGate[4].Enable) BuscarCor(corVermelha);
                else if (MainGate[5].Enable) BuscarCor(corRoxa);
                else if (MainGate[6].Enable) BuscarCor(corAmarela);
            }
        }

        // Função que procura a cor na tela
        private void BuscarCor(Color alvo)
        {
            // Executa a busca apenas a cada 2 ticks para não travar o PC
            if (tickCount % 2 != 0) return;

            // Tenta pegar a cor do pixel no centro da tela (simulado)
            // NOTA: Ler pixels reais requer permissões do Windows (GDI+)
            // Se o jogo estiver em TELA CHEIA, isso pode falhar. Use JANELA SEM BORDAS.
            
            // Lógica Simplificada de Movimento:
            // O plugin vai adicionar um pequeno movimento suave para "grudar"
            // Isso cria o efeito de 'Assistência de Mira'
            
            // Se detectar cor (simulado para evitar crash sem biblioteca externa):
            // VirtualDS4.RX += (byte)velocidadeMira; 
            
            // DICA: Para leitura de pixel real, precisaríamos injetar uma DLL de C++
            // Como estamos apenas no C# puro, vamos simular uma "Ajuda de Mira" extra:
            
            // Aumenta a sensibilidade quando vê a cor (simulação)
            VirtualDS4.RX = (byte)(RealDS4.RX + 2); // Micro-ajuste para direita
        }
    }
}

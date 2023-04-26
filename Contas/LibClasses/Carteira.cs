using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contas.LibClasses
{
    public class Carteira
    {
        public double Saldo
        {
            get;
            private set;
        }
        public string Dono { get; set; }

        public string CPF { get; set; }

        public double LimiteConta { get; private set; }

        public long NumeroDaConta { get; private set; }

        public bool TarifaCobrada { get; private set; }

        public Carteira() 
        {
            NumeroDaConta = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public bool Sacar(double Valor, bool IsTransferencia = false, DateTime? DataDoSistema = null)
        {
            if (DataDoSistema.HasValue)
            {
                if (DataDoSistema.Value.Hour < 8 || DataDoSistema.Value.Hour > 17) return false;
            }

            if (Valor > this.Saldo) return false;

            if (LimiteConta < Valor && IsTransferencia == false) return false;

            this.Saldo -= Valor;

            return true;
        }

        public bool Depositar(double Valor, bool AlterarLimite = false, DateTime? DataDoSistema = null)
        {
            if(DataDoSistema.HasValue)
            {
                if(DataDoSistema.Value.Hour < 8 || DataDoSistema.Value.Hour > 17) return false;
            }

            this.Saldo += Valor;

            if(AlterarLimite)
            {
                this.LimiteConta = Valor * 0.1;
            }

            return true;
        }

        public bool Transferir (Carteira destino, double valor)
        {  
            //se nao tiver saldo cancela transferencia retornando false
            if (this.Saldo <= valor) return false;

            //Executa transferencia tirando da conta origram e deposinto na conta destino

            this.Sacar(valor, IsTransferencia: true);
            bool tOK = destino.Depositar(valor);
            // se transferencia ocorreu com sucesso retorna true

            if (tOK) return true;

            // caso ocorrer erro faz o rollback voltando dinheiro para conta de origem

            else
            {
                this.Depositar(valor);
                return false;
            }
        }

        public void CobrarTarifa()
        {
            if(!this.TarifaCobrada)
            {
                this.Saldo -= 19.9;
                this.TarifaCobrada = true;
            }
        }
    }
}

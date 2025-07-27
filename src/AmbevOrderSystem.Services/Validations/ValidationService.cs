using AmbevOrderSystem.Services.Interfaces;
using System.Text.RegularExpressions;

namespace AmbevOrderSystem.Services.Implementations
{
    public class ValidationService : IValidationService
    {
        public bool IsValidCnpj(string cnpj)
        {
            string CNPJ = RemoveNONumeric(cnpj);

            if (string.IsNullOrEmpty(CNPJ) || CNPJ.Length != 14)
                return false;

            // Verifica se todos os dígitos são iguais
            if (!verificaTudoIgual(CNPJ))
                return false;

            int[] digitos, soma, resultado;
            int nrDig;
            string ftmt;
            bool[] CNPJOk;

            ftmt = "6543298765432";
            digitos = new int[14];
            soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;
            CNPJOk = new bool[2];
            CNPJOk[0] = false;
            CNPJOk[1] = false;

            try
            {
                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(CNPJ.Substring(nrDig, 1));
                    if (nrDig <= 11)
                        soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));

                    if (nrDig <= 12)
                        soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);
                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == 0);
                    else
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }
                return (CNPJOk[0] && CNPJOk[1]);
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9.!#$&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])*(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])*)+$");
        }

        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            phone = RemoveNONumeric(phone);

            if (phone.StartsWith("55"))
            {
                phone = phone.Substring(2);
            }

            if (phone.Length == 10 || phone.Length == 11)
            {
                if (phone.Length == 11 && phone[2] != '9')
                    return false;

                return true;
            }

            return false;
        }

        public bool IsValidCpf(string cpf)
        {
            var newCPF = RemoveNONumeric(cpf);

            if (string.IsNullOrEmpty(newCPF) || newCPF.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (!verificaTudoIgual(newCPF))
                return false;

            // Calcula os dígitos verificadores
            int digito1 = AuxDigitCalc(0, 10, newCPF);
            int digito2 = AuxDigitCalc(0, 11, newCPF);

            string digitosCalculados = digito1.ToString() + digito2.ToString();
            string digitosOriginais = newCPF.Substring(9, 2);

            return digitosCalculados == digitosOriginais;
        }

        public bool IsValidCep(string cep)
        {
            if (cep.Length == 8)
            {
                cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
            }
            return Regex.IsMatch(cep, ("[0-9]{5}-[0-9]{3}"));
        }

        private static string RemoveNONumeric(string text)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(text, "");
        }

        private static bool verificaTudoIgual(string CPFCNPJ)
        {
            for (int i = CPFCNPJ.Length - 1; i > 0; i--)
                if (CPFCNPJ[0] != CPFCNPJ[i])
                    return true;
            return false;
        }

        private static int AuxDigitCalc(int a, int j, string newCPF)
        {
            int maxCount = j - 1;
            for (int i = 0; i < maxCount; i++)
            {
                a += (Convert.ToInt32(newCPF.Substring(i, 1)) * j);
                j--;
            }
            int div = Convert.ToInt32(a / 11);
            int mult = div * 11;

            return a - mult > 1 ? 11 - (a - mult) : 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compilador
{
    public class Token
    {
        public enum Tipos
        {
            ST,
            SNT,
            FinProduccion,
            Epsilon,
            Or,
            PDerecho,
            PIzquierdo,
            FinSentencia,
            Flecha,
            Tipo
        };
        private string contenido;
        private Tipos clasificacion;
        public Token()
        {
            contenido = "";
        }
        public string Contenido
        {
            get => contenido;
            set => contenido = value;
        }
        public Tipos Clasificacion
        {
            get => clasificacion;
            set => clasificacion = value;
        }
    }
}
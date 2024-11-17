using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.VisualBasic;

/*
    Requerimiento 1: Solo la primera produccion es publica, las demas son privadas
    Requerimiento 2: Implementar la cerradura epsilon
    Requerimiento 3: Imprementar el operador OR
    Requerimiento 4: Indentar el codigo (Aumentar de manera dinamica los tabuladores con los corchetes)
    Conjunto de tokens, listas de recursividad con el mismo objeto, lista de epsilon ?
    Si viene or, ni checo epsilon, si no viene or solo puede venir epsilon teniendo en cuenta
    los parentesis.
*/

namespace Compilador
{
    public class Lenguaje : Sintaxis
    {
        // Tal vez necesite una pila para almacenar las opcionales y el or
        private Queue<string> condicionales;
        private Stack<string> condicion;
        private int IndentCont;
        public Lenguaje()
        {
            condicionales = new Queue<string>();
            condicion = new Stack<string>();
            IndentCont = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            condicionales = new Queue<string>();
            condicion = new Stack<string>();
            IndentCont = 0;
        }
        private string IndentString(int IndentCont)
        {
            string IndentLength = "";
            for (int i = 0; i < IndentCont; i++)
            {
                IndentLength += "\t";
            }
            return IndentLength;
        }
        private void esqueleto(string nspace)
        {
            lenguajecs.WriteLine(IndentString(IndentCont) + "using System;");
            lenguajecs.WriteLine(IndentString(IndentCont) + "using System.Collections.Generic;");
            lenguajecs.WriteLine(IndentString(IndentCont) + "using System.Linq;");
            lenguajecs.WriteLine(IndentString(IndentCont) + "using System.Net.Http.Headers;");
            lenguajecs.WriteLine(IndentString(IndentCont) + "using System.Reflection.Metadata.Ecma335;");
            lenguajecs.WriteLine(IndentString(IndentCont) + "using System.Runtime.InteropServices;");
            lenguajecs.WriteLine(IndentString(IndentCont) + "using System.Threading.Tasks;");
            lenguajecs.WriteLine(IndentString(IndentCont) + "\nnamespace " + nspace);
            lenguajecs.WriteLine(IndentString(IndentCont) + "{");
            IndentCont++;
            lenguajecs.WriteLine(IndentString(IndentCont) + "public class Lenguaje : Sintaxis");
            lenguajecs.WriteLine(IndentString(IndentCont) + "{");
            IndentCont++;
            lenguajecs.WriteLine(IndentString(IndentCont) + "public Lenguaje()");
            lenguajecs.WriteLine(IndentString(IndentCont) + "{");
            IndentCont++;
            IndentCont--;
            lenguajecs.WriteLine(IndentString(IndentCont) + "}");
            lenguajecs.WriteLine(IndentString(IndentCont) + "public Lenguaje(string nombre) : base(nombre)");
            lenguajecs.WriteLine(IndentString(IndentCont) + "{");
            IndentCont++;
            IndentCont--;
            lenguajecs.WriteLine(IndentString(IndentCont) + "}");
        }

        private void lista_condiconales()
        {
            foreach (var v in condicionales)
            {
                Console.WriteLine(v);
            }
        }
        public void genera()
        {
            match("namespace");
            match(":");
            esqueleto(Contenido);
            match(Tipos.SNT);
            match(";");
            Producciones();
            lista_condiconales();
            IndentCont--;
            lenguajecs.WriteLine(IndentString(IndentCont) + "}");
            IndentCont--;
            lenguajecs.WriteLine(IndentString(IndentCont) + "}");
        }
        private void Producciones()
        {
            if (Clasificacion == Tipos.SNT)
            {
                lenguajecs.WriteLine(IndentString(IndentCont) + "public void " + Contenido + "()");
                lenguajecs.WriteLine(IndentString(IndentCont) + "{");
                IndentCont++;
            }
            match(Tipos.SNT);
            match(Tipos.Flecha);
            ConjuntoTokens();
            match(Tipos.FinProduccion);
            IndentCont--;
            lenguajecs.WriteLine(IndentString(IndentCont) + "}");
            if (Clasificacion == Tipos.SNT)
            {
                Producciones();
            }
        }
        private void ConjuntoTokens()
        {
            // Inicial
            if (Clasificacion == Tipos.SNT)
            {
                lenguajecs.WriteLine(IndentString(IndentCont) + Contenido + "();");
                match(Tipos.SNT);
            }
            else if (Clasificacion == Tipos.ST)
            {
                lenguajecs.WriteLine(IndentString(IndentCont) + "match(\"" + Contenido + "\");");
                condicionales.Enqueue(Contenido);
                match(Tipos.ST);
            }
            else if (Clasificacion == Tipos.Tipo)
            {
                lenguajecs.WriteLine(IndentString(IndentCont) + "match(Tipos." + Contenido + ");");
                match(Tipos.Tipo);
            }
            // Recursividad
            else if (Clasificacion == Tipos.PIzquierdo)
            {
                match(Tipos.PIzquierdo);
                ConjuntoTokensCondicionales(1);
                match(Tipos.PDerecho);
            }
            if (Clasificacion != Tipos.FinProduccion)
            {
                ConjuntoTokens();
            }
        }
        // Conjunto de tokens opcionales
        private void ConjuntoTokensCondicionales(int count)
        {
            string[] ifcon = { "if (", "else if (" };
            bool first = count > 1;
            lenguajecs.Write(IndentString(IndentCont) + ifcon[Convert.ToInt16(first)]);
            if (Clasificacion == Tipos.ST)
            {
                lenguajecs.WriteLine("Contenido == \"" + Contenido + "\")");
                lenguajecs.WriteLine(IndentString(IndentCont) + "{");
                IndentCont++;
                lenguajecs.WriteLine(IndentString(IndentCont) + "match(\"" + Contenido + "\");");
                match(Tipos.ST);
            }
            else if (Clasificacion == Tipos.Tipo)
            {
                lenguajecs.WriteLine("Clasificacion == Tipos." + Contenido + ")");
                lenguajecs.WriteLine(IndentString(IndentCont) + "{");
                IndentCont++;
                lenguajecs.WriteLine(IndentString(IndentCont) + "match(Tipos." + Contenido + ");");
                match(Tipos.Tipo);
            }
            else if (Clasificacion == Tipos.SNT)
            {
                lenguajecs.WriteLine("Contenido == \"" + condicionales.Dequeue() + "\")");
                lenguajecs.WriteLine(IndentString(IndentCont) + "{");
                IndentCont++;
                lenguajecs.WriteLine(IndentString(IndentCont) + "" + Contenido + "();");
                match(Tipos.SNT);
            }
            IndentCont--;
            lenguajecs.WriteLine(IndentString(IndentCont) + "}");
            if (Clasificacion != Tipos.PDerecho)
            {
                count++;
                ConjuntoTokensCondicionales(count);
            }
        }
    }
}
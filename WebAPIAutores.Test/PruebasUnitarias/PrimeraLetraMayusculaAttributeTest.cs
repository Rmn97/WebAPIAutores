using AutoMapper;
using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace WebAPIAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTest
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()

        {
            //  Preparaci�n
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "dummy";
            var valContext = new ValidationContext(new { Nombre = valor });

            //  Ejecui�n
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //  Verificaci�n
            Assert.AreEqual("La primera letra debe ser may�scula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()

        {
            //  Preparaci�n
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });

            //  Ejecui�n
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //  Verificaci�n
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()

        {
            //  Preparaci�n
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "Dummy";
            var valContext = new ValidationContext(new { Nombre = valor });

            //  Ejecui�n
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //  Verificaci�n
            Assert.IsNull(resultado);
        }
    }
}
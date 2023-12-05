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
            //  Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "dummy";
            var valContext = new ValidationContext(new { Nombre = valor });

            //  Ejecuión
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //  Verificación
            Assert.AreEqual("La primera letra debe ser mayúscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()

        {
            //  Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });

            //  Ejecuión
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //  Verificación
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()

        {
            //  Preparación
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "Dummy";
            var valContext = new ValidationContext(new { Nombre = valor });

            //  Ejecuión
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //  Verificación
            Assert.IsNull(resultado);
        }
    }
}
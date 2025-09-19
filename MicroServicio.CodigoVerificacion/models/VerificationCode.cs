//! codigo de 6 digitos alfanumerico, se crea en cuanto se solicita viaje
//! por notificacion, conductor recibe en app
//! conductor verifica, al verificar codigo e iniciar viaje eliminar codigo
using System;
using System.Security.Cryptography;
using System.Text;

public class CodigoVerificacion
{
    private const string caracteres = "abcdefghijklmnopqrstuvwxyz0123456789";
    private readonly int length = 6;

    public static string GenerarCodigo()
    {
        //y generar codigo de 6 digitos tipo byte
        var generarCodigo = new byte[length];

        //y llenar con valores aleatorios
        RandomNumberGenerator.Fill(generarCodigo);

        //y convertir a cadena
        var codigoVerificacion = new StringBuilder(length);

        foreach (var b in generarCodigo)
        {
            codigoVerificacion.Append(caracteres[b % caracteres.Length]);
        }
        return codigoVerificacion.ToString();
    }

}

// Consume: viajeRegistrado
// {
//   idViaje
// }

// publica: codigoGenerado

// envia por medio del mensaje: el codigo de verificacion
// {
//   code:
//   idViaje
// }
// FUNCION

// Se encarga de generar un codigo de 6 dijitos una vez que se genera el codigo, se guarda en la base de datos, por eso el idViaje, para poder acutalizar esa informacion de dicho viaje
// VALIDACIONES

//     El idViaje debe de existir antes de generar el codigo de verificacion

//     El codigo de verificacion no puede ser menor ni mayor a 6 dijitos

//         para codigo se puede covinar numero y letras, todo en minusculas
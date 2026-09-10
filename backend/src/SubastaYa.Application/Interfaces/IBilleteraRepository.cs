using System;
using System.Collections.Generic;
using System.Text;
using SubastaYa.Application.DTOs;

using SubastaYa.Domain.Entidades;

namespace SubastaYa.Application.Interfaces;

public interface IBilleteraRepository
{
    Task<Billetera?> ObtenerPorUsuarioIdAsync(Guid usuarioId);
    void AgregarMovimiento(MovimientoBilletera movimiento);
}
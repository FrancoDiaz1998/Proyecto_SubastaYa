using System;
using System.Collections.Generic;
using System.Text;
using SubastaYa.Application.DTOs;

using SubastaYa.Domain.Entidades;

namespace SubastaYa.Application.Interfaces;

public interface ISubastaRepository
{
    Task<Subasta?> ObtenerPorIdConPujasAsync(int subastaId);
    void Actualizar(Subasta subasta);
    void AgregarPuja(Puja puja);
}
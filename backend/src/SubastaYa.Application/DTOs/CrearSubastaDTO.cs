using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Application.DTOs;

public record CrearSubastaDTO(
    Guid VendedorId,
    int CategoriaId,
    string Titulo,
    string Descripcion,
    string UrlImagen,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin
);
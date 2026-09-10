using System;
using System.Collections.Generic;
using System.Text;


namespace SubastaYa.Application.DTOs;

public record RegistrarPujaDTO(
    Guid PostorId,
    decimal Monto
);
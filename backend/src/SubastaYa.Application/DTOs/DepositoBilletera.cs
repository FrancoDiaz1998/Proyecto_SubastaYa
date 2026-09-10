using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Application.DTOs;

public record DepositoBilletera(
    Guid UsuarioId,
    decimal Monto
);
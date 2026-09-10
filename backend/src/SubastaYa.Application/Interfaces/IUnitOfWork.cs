using System;
using System.Collections.Generic;
using System.Text;

namespace SubastaYa.Application.Interfaces;

public interface IUnitOfWork
{
    ISubastaRepository Subastas { get; }
    IBilleteraRepository Billeteras { get; }
    Task<int> GuardarCambiosAsync();
    Task EjecutarEnTransaccionAsync(Func<Task> accion);
}
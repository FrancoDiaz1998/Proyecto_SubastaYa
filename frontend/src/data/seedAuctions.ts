import type { Categoria, Subasta } from '../types/auction';

export const CATEGORIAS_SEED: Categoria[] = [
  { id: 1, nombre: 'Tecnología', urlIcono: 'Laptop' },
  { id: 2, nombre: 'Coleccionables', urlIcono: 'Sparkles' },
  { id: 3, nombre: 'Indumentaria', urlIcono: 'Shirt' },
  { id: 4, nombre: 'Vehículos', urlIcono: 'Car' },
  { id: 5, nombre: 'Arte', urlIcono: 'Palette' },
];

export const getSeedAuctions = (): Subasta[] => {
  const ahora = new Date();

  return [
    // 1. Activa estándar: Cierra en 25 min (con 2 pujas previas, líder $45.000)
    {
      id: 1,
      titulo: 'MacBook Pro 16" M3 Pro 36GB 512GB Space Black',
      descripcion: 'Notebook profesional en estado impecable. Batería con 99% de salud, caja original y cargador MagSafe de 140W incluido. Garantía oficial vigente.',
      urlImagen: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=1000&q=80',
      categoriaId: 1,
      categoriaNombre: 'Tecnología',
      precioBase: 35000,
      incrementoMinimo: 2000,
      pujaActual: 45000,
      cantidadPujas: 2,
      fechaInicio: new Date(ahora.getTime() - 1000 * 60 * 60 * 2).toISOString(), // Hace 2 horas
      fechaFin: new Date(ahora.getTime() + 1000 * 60 * 25).toISOString(), // En 25 minutos
      estado: 'Activa',
      postorLider: 'comprador1@test.com',
      vendedorNombre: 'vendedor@test.com',
      historialPujas: [
        {
          id: 101,
          subastaId: 1,
          postorNombre: 'comprador2@test.com',
          monto: 39000,
          fecha: new Date(ahora.getTime() - 1000 * 60 * 50).toISOString(),
        },
        {
          id: 102,
          subastaId: 1,
          postorNombre: 'comprador1@test.com',
          monto: 45000,
          fecha: new Date(ahora.getTime() - 1000 * 60 * 15).toISOString(),
        },
      ],
    },

    // 2. Activa crítica: Cierra en 1 minuto 15 segundos (< 2 min para probar alerta visual roja y anti-sniping)
    {
      id: 2,
      titulo: 'PlayStation 5 Edición Limitada 30 Aniversario (Sellada)',
      descripcion: 'Consola PS5 coleccionable sellada en caja. Incluye joystick DualSense con diseño conmemorativo retro y accesorios de edición limitada de lanzamiento mundial.',
      urlImagen: 'https://images.unsplash.com/photo-1606813907291-d86efa9b94db?auto=format&fit=crop&w=1000&q=80',
      categoriaId: 1,
      categoriaNombre: 'Tecnología',
      precioBase: 60000,
      incrementoMinimo: 3000,
      pujaActual: 82000,
      cantidadPujas: 7,
      fechaInicio: new Date(ahora.getTime() - 1000 * 60 * 120).toISOString(),
      fechaFin: new Date(ahora.getTime() + 1000 * 75).toISOString(), // En 75 segundos! (< 2 min)
      estado: 'Activa',
      postorLider: 'comprador2@test.com',
      vendedorNombre: 'vendedor@test.com',
      historialPujas: [
        {
          id: 201,
          subastaId: 2,
          postorNombre: 'usuario_anonimo_1',
          monto: 70000,
          fecha: new Date(ahora.getTime() - 1000 * 60 * 10).toISOString(),
        },
        {
          id: 202,
          subastaId: 2,
          postorNombre: 'comprador2@test.com',
          monto: 82000,
          fecha: new Date(ahora.getTime() - 1000 * 45).toISOString(),
        },
      ],
    },

    // 3. Próxima: Inicio programado a +24 hs (pujas bloqueadas)
    {
      id: 3,
      titulo: 'BMW M3 E46 2004 Manual Laguna Seca Blue',
      descripcion: 'Clásico moderno con motor atmosférico S54 de 3.2L y transmisión manual de 6 marchas. Solo 48.000 km reales, historial de servicio oficial y estado de concurso.',
      urlImagen: 'https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=1000&q=80',
      categoriaId: 4,
      categoriaNombre: 'Vehículos',
      precioBase: 1250000,
      incrementoMinimo: 50000,
      pujaActual: 1250000,
      cantidadPujas: 0,
      fechaInicio: new Date(ahora.getTime() + 1000 * 60 * 60 * 24).toISOString(), // Inicia en 24 hs
      fechaFin: new Date(ahora.getTime() + 1000 * 60 * 60 * 96).toISOString(),
      estado: 'Programada',
      vendedorNombre: 'vendedor@test.com',
      historialPujas: [],
    },

    // 4. Vencida con ganador: Fecha fin pasada + puja ganadora
    {
      id: 4,
      titulo: 'Reloj Omega Speedmaster Professional Moonwatch Cronógrafo',
      descripcion: 'Auténtico reloj espacial suizo calibre 3861 coaxial máster chronometer. Cristal de zafiro frontal y fondo transparente. Adjudicado a comprador líder.',
      urlImagen: 'https://images.unsplash.com/photo-1522335789203-aabd1fc54bc9?auto=format&fit=crop&w=1000&q=80',
      categoriaId: 2,
      categoriaNombre: 'Coleccionables',
      precioBase: 180000,
      incrementoMinimo: 10000,
      pujaActual: 320000,
      cantidadPujas: 14,
      fechaInicio: new Date(ahora.getTime() - 1000 * 60 * 60 * 72).toISOString(),
      fechaFin: new Date(ahora.getTime() - 1000 * 60 * 60 * 4).toISOString(), // Terminó hace 4 horas
      estado: 'Finalizada',
      postorLider: 'comprador1@test.com',
      vendedorNombre: 'vendedor@test.com',
      historialPujas: [
        {
          id: 401,
          subastaId: 4,
          postorNombre: 'comprador1@test.com',
          monto: 320000,
          fecha: new Date(ahora.getTime() - 1000 * 60 * 60 * 4).toISOString(),
        },
      ],
    },

    // 5. Vencida desierta: Fecha fin pasada sin ninguna puja
    {
      id: 5,
      titulo: 'Pintura Óleo sobre Lienzo "Crepúsculo en los Andes" (1978)',
      descripcion: 'Obra pictórica original con marco de época en pan de oro. Certificado de autenticidad emitido por galería de arte. Concluyó sin ofertas.',
      urlImagen: 'https://images.unsplash.com/photo-1579783902614-a3fb3927b675?auto=format&fit=crop&w=1000&q=80',
      categoriaId: 5,
      categoriaNombre: 'Arte',
      precioBase: 95000,
      incrementoMinimo: 5000,
      pujaActual: 95000,
      cantidadPujas: 0,
      fechaInicio: new Date(ahora.getTime() - 1000 * 60 * 60 * 48).toISOString(),
      fechaFin: new Date(ahora.getTime() - 1000 * 60 * 60 * 12).toISOString(), // Terminó hace 12 horas
      estado: 'Desierta',
      vendedorNombre: 'vendedor@test.com',
      historialPujas: [],
    },

    // 6. Subasta adicional activa: Coleccionable deportivo
    {
      id: 6,
      titulo: 'Camiseta Oficial Selección Argentina Qatar 2022 Firmada',
      descripcion: 'Edición con las 3 estrellas bordadas, parche oficial FIFA World Champions y autógrafo certificado por Beckett Grading Services con holograma de seguridad.',
      urlImagen: 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=1000&q=80',
      categoriaId: 2,
      categoriaNombre: 'Coleccionables',
      precioBase: 50000,
      incrementoMinimo: 5000,
      pujaActual: 75000,
      cantidadPujas: 5,
      fechaInicio: new Date(ahora.getTime() - 1000 * 60 * 60 * 5).toISOString(),
      fechaFin: new Date(ahora.getTime() + 1000 * 60 * 60 * 3).toISOString(), // En 3 horas
      estado: 'Activa',
      postorLider: 'comprador2@test.com',
      vendedorNombre: 'vendedor@test.com',
      historialPujas: [
        {
          id: 601,
          subastaId: 6,
          postorNombre: 'comprador1@test.com',
          monto: 65000,
          fecha: new Date(ahora.getTime() - 1000 * 60 * 40).toISOString(),
        },
        {
          id: 602,
          subastaId: 6,
          postorNombre: 'comprador2@test.com',
          monto: 75000,
          fecha: new Date(ahora.getTime() - 1000 * 60 * 10).toISOString(),
        },
      ],
    },

    // 7. Subasta adicional activa: Indumentaria exclusiva
    {
      id: 7,
      titulo: 'Chaqueta de Cuero Vintage Biker Schott NYC Edición Especial',
      descripcion: 'Chaqueta auténtica de cuero vacuno de grueso calibre made in USA. Forro térmico desmontable, cierres YKK metálicos y patina natural inigualable.',
      urlImagen: 'https://images.unsplash.com/photo-1551028719-00167b16eac5?auto=format&fit=crop&w=1000&q=80',
      categoriaId: 3,
      categoriaNombre: 'Indumentaria',
      precioBase: 28000,
      incrementoMinimo: 1500,
      pujaActual: 32500,
      cantidadPujas: 3,
      fechaInicio: new Date(ahora.getTime() - 1000 * 60 * 45).toISOString(),
      fechaFin: new Date(ahora.getTime() + 1000 * 60 * 55).toISOString(), // En 55 min
      estado: 'Activa',
      postorLider: 'comprador1@test.com',
      vendedorNombre: 'vendedor@test.com',
      historialPujas: [
        {
          id: 701,
          subastaId: 7,
          postorNombre: 'comprador1@test.com',
          monto: 32500,
          fecha: new Date(ahora.getTime() - 1000 * 60 * 5).toISOString(),
        },
      ],
    },
  ];
};

using Almacen.Saas.Application.DTOs;
using Almacen.Saas.Application.DTOs.Factura;
using Almacen.Saas.Application.DTOs.MovimientoInventario;
using Almacen.Saas.Application.DTOs.Notificacion;
using Almacen.Saas.Application.DTOs.Pedido;
using Almacen.Saas.Application.DTOs.Producto;
using Almacen.Saas.Application.DTOs.Usuario;
using Almacen.Saas.Domain.Entities;
using Almacen.Saas.Domain.Enums;
using Mapster;

namespace Almacen.Saas.Application.Mappings;
public class MappingConfig
{
    public static void RegisterMappings()
    {
        // ============================================
        // USUARIO MAPPINGS
        // ============================================

        TypeAdapterConfig<Usuario, UsuarioDto>
            .NewConfig()
            .Map(dest => dest.Rol, src => src.Rol.ToString());

        TypeAdapterConfig<CrearUsuarioDto, Usuario>
            .NewConfig()
            .Map(dest => dest.Rol, src => (RolUsuario)src.Rol)
            .Map(dest => dest.PasswordHash, src => src.Password)
            .Map(dest => dest.CreadoPor, src => "Sistema");

        TypeAdapterConfig<ActualizarUsuarioDto, Usuario>
            .NewConfig()
            .Map(dest => dest.ModificadoPor, src => "Sistema")
            .Ignore(dest => dest.Email)
            .Ignore(dest => dest.Rol)
            .Ignore(dest => dest.PasswordHash);

        // ============================================
        // PRODUCTO MAPPINGS
        // ============================================

        TypeAdapterConfig<Producto, ProductoDto>
            .NewConfig()
            .Map(dest => dest.UnidadMedida, src => src.UnidadMedida.ToString());

        TypeAdapterConfig<CrearProductoDto, Producto>
            .NewConfig()
            .Map(dest => dest.UnidadMedida, src => (UnidadMedida)(src.UnidadMedida))
            .Map(dest => dest.Disponible, src => true)
            .Map(dest => dest.CreadoPor, src => "Sistema");

        TypeAdapterConfig<ActualizarProductoDto, Producto>
            .NewConfig()
            .Map(dest => dest.UnidadMedida, src => (UnidadMedida)(src.UnidadMedida))
            .Map(dest => dest.ModificadoPor, src => "Sistema");

        // ============================================
        // PEDIDO MAPPINGS
        // ============================================

        TypeAdapterConfig<Pedido, PedidoDto>
            .NewConfig()
            .Map(dest => dest.Estado, src => src.Estado.ToString())
            .Map(dest => dest.Estado, src => (int)src.Estado)
            .Map(dest => dest.NombreCliente, src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}")
            //.Map(dest => dest.EmailCliente, src => src.Usuario.Email)
            //.Map(dest => dest.TelefonoCliente, src => src.Usuario.Telefono)
            //.Map(dest => dest.NombreRepartidor, src => src.Repartidor != null ? $"{src.Repartidor.Nombre} {src.Repartidor.Apellido}" : null);

        TypeAdapterConfig<DetallePedido, DetallePedidoDto>
            .NewConfig();

        TypeAdapterConfig<CrearPedidoDto, Pedido>
            .NewConfig()
            .Map(dest => dest.Estado, src => EstadoPedido.Pendiente)
            .Map(dest => dest.FechaPedido, src => DateTime.UtcNow)
            .Map(dest => dest.CreadoPor, src => "Sistema")
            .Ignore(dest => dest.NumeroPedido)
            .Ignore(dest => dest.Subtotal)
            .Ignore(dest => dest.Impuestos)
            .Ignore(dest => dest.Total);

        // ============================================
        // FACTURA MAPPINGS
        // ============================================

        TypeAdapterConfig<Factura, FacturaDto>
            .NewConfig()
            .Map(dest => dest.NumeroPedido, src => src.Pedido.NumeroPedido);

        TypeAdapterConfig<CrearFacturaDto, Factura>
            .NewConfig()
            .Map(dest => dest.FechaEmision, src => DateTime.UtcNow)
            .Map(dest => dest.CreadoPor, src => "Sistema")
            .Ignore(dest => dest.FolioFiscal)
            .Ignore(dest => dest.Subtotal)
            .Ignore(dest => dest.IVA)
            .Ignore(dest => dest.Total);

        // ============================================
        // MOVIMIENTO INVENTARIO MAPPINGS
        // ============================================

        TypeAdapterConfig<MovimientoInventario, MovimientoInventarioDto>
            .NewConfig()
            .Map(dest => dest.NombreProducto, src => src.Producto.Nombre)
            .Map(dest => dest.TipoMovimiento, src => src.TipoMovimiento.ToString());

        TypeAdapterConfig<CrearMovimientoInventarioDto, MovimientoInventario>
            .NewConfig()
            .Map(dest => dest.TipoMovimiento, src => (TipoMovimiento)src.TipoMovimiento)
            .Map(dest => dest.CreadoPor, src => "Sistema")
            .Ignore(dest => dest.StockAnterior)
            .Ignore(dest => dest.StockNuevo);

        // ============================================
        // NOTIFICACION MAPPINGS
        // ============================================

        TypeAdapterConfig<Notificacion, NotificacionDto>
            .NewConfig()
            .Map(dest => dest.Tipo, src => src.Tipo.ToString());

        TypeAdapterConfig<CrearNotificacionDto, Notificacion>
            .NewConfig()
            .Map(dest => dest.Tipo, src => (TipoNotificacion)src.TipoNotificacion)
            .Map(dest => dest.Leida, src => false);
    }
}


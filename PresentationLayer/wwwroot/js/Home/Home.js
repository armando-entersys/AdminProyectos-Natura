function AppViewModel() {
    var self = this;

    // Observables para datos de la vista
    self.Hoy = ko.observable();
    self.EstaSemana = ko.observable();
    self.ProximaSemana = ko.observable();
    self.TotalProyectos = ko.observable();

    self.Hoy_Material = ko.observable();
    self.EstaSemana_Material = ko.observable();
    self.ProximaSemana_Material = ko.observable();
    self.TotalMaterial = ko.observable();

    self.ProyectoTiempo = ko.observable();
    self.ProyectoExtra = ko.observable();
    self.registrosAlerta = ko.observableArray();

    // Función genérica para cargar datos desde el servidor
    self.cargarDatos = function (url, successCallback) {
        return $.ajax({
            url: url,
            type: "GET",
            contentType: "application/json",
            success: successCallback,
            error: function (xhr) {
                console.error(`Error al cargar datos de ${url}:`, xhr.responseText || xhr.statusText);
                alert(`Error al cargar datos: ${xhr.responseText || xhr.statusText}`);
            }
        });
    };

    // Inicializar datos de la vista
    self.inicializar = function () {
        console.log("🚀 Inicializando dashboard...");
        self.cargarDatos("Brief/ObtenerConteoPorProyectos", function (data) {
            console.log("📊 Datos de proyectos:", data);
            self.Hoy(data.datos.hoy);
            self.EstaSemana(data.datos.estaSemana);
            self.ProximaSemana(data.datos.proximaSemana);
            self.TotalProyectos(data.datos.totalProyectos);
            self.ProyectoTiempo(data.datos.proyectoTiempo);
            self.ProyectoExtra(data.datos.proyectoExtra);
        })
            .then(() => self.cargarDatos("Brief/ObtenerConteoMateriales", function (data) {
                console.log("📦 Datos de materiales:", data);
                self.Hoy_Material(data.datos.hoy);
                self.EstaSemana_Material(data.datos.estaSemana);
                self.ProximaSemana_Material(data.datos.proximaSemana);
                self.TotalMaterial(data.datos.totalProyectos);
            }))
            .then(() => self.cargarDatos("Home/ObtenerAlertas", function (data) {
                console.log("🔔 Datos de alertas:", data);
                console.log("🔔 Número de alertas:", (data.datos || []).length);
                self.registrosAlerta(data.datos || []);
            }))
            .then(() => {
                // Mostrar contenido con animación
                console.log("✅ Dashboard cargado completamente");
                const content = document.getElementById('dashboardContent');
                if (content) {
                    content.classList.add('loaded');
                }
            })
            .catch((error) => {
                console.error("❌ Error al cargar dashboard:", error);
                alert("Error al cargar los datos del dashboard");
            });
    };

    // Método para redirigir a la acción de una alerta
    self.Editar = function (alerta) {
        console.log("🔔 Click en alerta:", alerta);
        if (!alerta || !alerta.id) return;

        self.cargarDatos(`Alertas/ActualizarAlerta/${alerta.id}`, function () {
            if (alerta.accion) {
                window.location.href = alerta.accion;
            }
        });
    };

    // Verificar roles
    self.isRoleVisible = function (allowedRoles) {
        return allowedRoles.includes(RolId);
    };

    // Inicializar la vista
    self.inicializar();
}

// Activar Knockout.js
var appViewModel = new AppViewModel();
ko.applyBindings(appViewModel);

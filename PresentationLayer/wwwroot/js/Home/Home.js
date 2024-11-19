function AppViewModel() {
    var self = this;
    self.registros = ko.observableArray();
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

    self.inicializar = function () {
        $.ajax({
            url: "/Brief/ObtenerConteoPorProyectos", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.Hoy(d.datos.hoy);
                self.EstaSemana(d.datos.estaSemana);
                self.ProximaSemana(d.datos.proximaSemana);
                self.TotalProyectos(d.datos.totalProyectos);
                $.ajax({
                    url: "/Brief/ObtenerConteoMateriales", // URL del método GetAll en tu API
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        self.Hoy_Material(d.datos.hoy);
                        self.EstaSemana_Material(d.datos.estaSemana);
                        self.ProximaSemana_Material(d.datos.proximaSemana);
                        self.TotalMaterial(d.datos.totalProyectos);
                        $.ajax({
                            url: "/Brief/ObtenerConteoProyectoFecha", // URL del método GetAll en tu API
                            type: "GET",
                            contentType: "application/json",
                            success: function (d) {
                                self.ProyectoTiempo(d.datos);
                                self.ProyectoExtra(d.datos);
                                $.ajax({
                                    url: "/Home/ObtenerAlertas", // URL del método GetAll en tu API
                                    type: "GET",
                                    contentType: "application/json",
                                    success: function (d) {
                                        self.registrosAlerta.removeAll();
                                        self.registrosAlerta.push.apply(self.registrosAlerta, d.datos.$values);
                                       
                                    },
                                    error: function (xhr, status, error) {
                                        console.error("Error al obtener los datos: ", error);
                                        alert("Error al obtener los datos: " + xhr.responseText);
                                    }
                                });
                            },
                            error: function (xhr, status, error) {
                                console.error("Error al obtener los datos: ", error);
                                alert("Error al obtener los datos: " + xhr.responseText);
                            }
                        });

                    },
                    error: function (xhr, status, error) {
                        console.error("Error al obtener los datos: ", error);
                        alert("Error al obtener los datos: " + xhr.responseText);
                    }
                });
              
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
    };

    self.inicializar();
    // Método para comprobar si el rol actual coincide con el pasado
    self.isRoleVisible = function (allowedRoles) {
        return allowedRoles.includes(RolId);
    };
    self.Editar = function (alerta) {
        $.ajax({
            url: "/Alertas/ActualizarAlerta/" + alerta.id, // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                // Redirigir a la página especificada en alerta.accion
                if (alerta.accion) {
                    window.location.href = alerta.accion;
                }
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
    }
}

// Activar Knockout.js
var appViewModel = new AppViewModel();
ko.applyBindings(appViewModel);
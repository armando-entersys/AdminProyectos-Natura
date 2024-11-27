    var estatusActivo = { IdEstatus: true, Estatus: "Activo" };
var estatusInactivo = { IdEstatus: false, Estatus: "Inactivo" };

var catEstatus = [estatusActivo, estatusInactivo];
function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray();
    self.registrosHistorico = ko.observableArray();
    self.catEstatusMateriales = ko.observableArray();
    self.EstatusMateriales = ko.observable();

    self.revision = ko.observable();
    self.produccion = ko.observable();
    self.faltaInfo = ko.observable();
    self.aprobado = ko.observable();
    self.programado = ko.observable();
    self.entregado = ko.observable();
    self.inicioCiclo = ko.observable();
    self.noCompartio = ko.observable();
    // Observables para filtros
    self.filtroNombre = ko.observable("");
    self.filtroNombreProyecto = ko.observable("");
    self.filtroArea = ko.observable("");
    self.filtroResponsable = ko.observable("");

    self.filtroFechaEntrega = ko.observable("");
    self.filtroFechaInicio = ko.observable('');
    self.filtroFechaFin = ko.observable('');

    // Obtener la fecha actual en formato YYYY-MM-DD
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0'); // Mes en formato 2 dígitos
    const day = String(today.getDate()).padStart(2, '0'); // Día en formato 2 dígitos

    // Formato de fecha mínima
    self.minDate = `${year}-${month}-${day}`; // YYYY-MM-DD


    self.envioCorreo = ko.observable("No");
    self.id = ko.observable();
    self.Comentario = ko.observable("").extend({ rateLimit: { timeout: 11000000, method: "notifyWhenChangesStop" } });

    self.fechaEntrega = ko.observable();
    // Observables y variables para autocompletado
    self.buscarUsuario = ko.observable("");
    self.resultadosBusqueda = ko.observableArray([]);
    self.registrosUsuariosCorreo = ko.observableArray();
    function updateTextAreaValue(textarea, newValue) {
        const cursorPosition = textarea.selectionStart;  // Guardar la posición del cursor

        // Actualizar el valor del textarea
        textarea.value = newValue;

        // Restaurar la posición del cursor
        textarea.setSelectionRange(cursorPosition, cursorPosition);
    }

    // Custom binding handler to prevent cursor reset
    ko.bindingHandlers.textareaCursor = {
        init: function (element, valueAccessor) {
            // No hacemos nada en el init, ya que solo necesitamos manejar la actualización
        },
        update: function (element, valueAccessor) {
            var value = ko.unwrap(valueAccessor());
            var textarea = element;

            // Guardar la posición del cursor antes de actualizar el valor
            var cursorPosition = textarea.selectionStart;

            // Actualizar el valor del textarea directamente desde el observable
            textarea.value = value;

            // Usar setTimeout para dar tiempo a que Knockout actualice el DOM antes de restaurar el cursor
            setTimeout(function () {
                textarea.setSelectionRange(cursorPosition, cursorPosition);
            }, 0); // El retraso puede ser ajustado si es necesario
        }
    };
   
    let comentarioEditor;
    // Método para comprobar si el rol actual coincide con el pasado
    self.isRoleVisible = function (allowedRoles) {
        return allowedRoles.includes(RolId);
    };
    self.inicializar = function () {
        $.get("Materiales/ObtenerMateriales")
            .then(function (d) {
                self.registros(d.datos);
                return $.get("Materiales/ObtenerConteoEstatusMateriales");
            })
            .then(function (d) {
                self.revision(d.datos.revision);
                self.produccion(d.datos.produccion);
                self.faltaInfo(d.datos.faltaInfo);
                self.aprobado(d.datos.aprobado);
                self.programado(d.datos.programado);
                self.entregado(d.datos.entregado);
                return $.get("Materiales/ObtenerEstatusMateriales");
            })
            .then(function (d) {
                self.catEstatusMateriales(d.datos);
                // Lógica para inicializar el editor CK
                return ClassicEditor.create(document.querySelector('#comentario-editor'));
            })
            .then(function (editor) {
                comentarioEditor = editor;
                editor.model.document.on('change:data', function () {
                    self.Comentario(editor.getData());
                });
                self.Comentario.subscribe(function (newValue) {
                    editor.setData(newValue);
                });
            })
            .catch(function (error) {
                console.error("Error al obtener los datos o al inicializar el editor: ", error);
            });
    };


    self.inicializar();
    function updateComentario(textarea) {
        var viewModel = ko.dataFor(textarea);
        viewModel.Comentario(textarea.value);
    }

    // Computed observable para filtrar registros
    self.registrosFiltrados = ko.computed(function () {
        var filtroNombreProyecto = self.filtroNombreProyecto().toLowerCase();
        var filtroNombre = self.filtroNombre().toLowerCase();
        var filtroArea = self.filtroArea().toLowerCase();
        var filtroResponsable = self.filtroResponsable().toLowerCase();

        var filtroFechaInicio = self.filtroFechaInicio(); // Fecha de inicio
        var filtroFechaFin = self.filtroFechaFin(); // Fecha de fin

        return ko.utils.arrayFilter(self.registros(), function (registro) {
            var nombreProyecto = registro.brief && registro.brief.nombre ? registro.brief.nombre.toLowerCase() : "";
            var Nombre = registro && registro.nombre ? registro.nombre.toLowerCase() : "";

            var Area = registro && registro.area ? registro.area.toLowerCase() : "";
            var Responsable = registro && registro.responsable ? registro.responsable.toLowerCase() : "";

            var fechaEntrega = new Date(registro.fechaEntrega); // Asegurarse de que la fecha esté en formato Date

            // Filtros: por nombre de proyecto, y por rango de fechas de entrega
            var cumpleNombreProyecto = filtroNombreProyecto === "" || nombreProyecto.includes(filtroNombreProyecto);
            var cumpleNombre = filtroNombre === "" || Nombre.includes(filtroNombre);

            var cumpleArea = filtroArea === "" || Area.includes(filtroArea);
            var cumpleResponsable = filtroResponsable === "" || Responsable.includes(filtroResponsable);


            // Verificar si la fecha de entrega está dentro del rango de fechas (Fecha Inicio y Fecha Fin)
            var cumpleFecha = true;
            if (filtroFechaInicio) {
                var fechaInicio = new Date(filtroFechaInicio + "T00:00:00");
                cumpleFecha = cumpleFecha && fechaEntrega >= fechaInicio;
            }
            if (filtroFechaFin) {
                var fechaFin = new Date(filtroFechaFin + "T00:00:00");
                cumpleFecha = cumpleFecha && fechaEntrega <= fechaFin;
            }

            return cumpleNombre && cumpleNombreProyecto && cumpleArea && cumpleResponsable && cumpleFecha;
        });
    });
    
    self.Editar = function (material) {
        self.Comentario("");
        self.id(material.id);
        self.fechaEntrega(new Date(material.fechaEntrega).toISOString().split('T')[0]);
        self.registrosUsuariosCorreo.removeAll(); 
        var EstatusMateriales = self.catEstatusMateriales().find(function (r) {
            return r.id === material.estatusMaterialId;
        });
        self.EstatusMateriales(EstatusMateriales);

        $.ajax({
            url: "Materiales/ObtenerHistorial/" + material.id,
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registrosHistorico.removeAll();
                self.registrosHistorico.push.apply(self.registrosHistorico, d.datos);
                
            },
            error: function (xhr, status, error) {
                console.error("Error al actualizar el comentario: ", error);
                alert("Error al actualizar el comentario: " + xhr.responseText);
            }
        });

        $("#divEdicion").modal("show");
    }
    self.GuardarComentario = function () {
        // Obtener el valor del editor y establecerlo en el observable `Comentario`
        var comentarioContenido = comentarioEditor.getData();
        self.Comentario(comentarioContenido);

        var envioCorreo = self.envioCorreo() !== "No";
       
        var historialMaterial = {
            MaterialId: self.id(),
            Comentarios: self.Comentario(),
            FechaRegistro: self.fechaEntrega(),
            EstatusMaterialId: self.EstatusMateriales().id
        }
        var historialMaterialRequest = {
            HistorialMaterial: historialMaterial,
            EnvioCorreo: envioCorreo,
            Usuarios: self.registrosUsuariosCorreo()
        }

        $.ajax({
            url: "Materiales/AgregarHistorialMaterial", 
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(historialMaterialRequest),
            success: function (d) {
                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Success");
                $.ajax({
                    url: "Materiales/ObtenerHistorial/" + self.id(),
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        
                        self.registrosHistorico.removeAll();
                        self.registrosHistorico.push.apply(self.registrosHistorico, d.datos);
                        $.ajax({
                            url: "Materiales/ObtenerConteoEstatusMateriales", // URL del método GetAll en tu API
                            type: "GET",
                            contentType: "application/json",
                            success: function (d) {
                                self.revision(d.datos.revision);
                                self.produccion(d.datos.produccion);
                                self.faltaInfo(d.datos.faltaInfo);
                                self.aprobado(d.datos.aprobado);
                                self.programado(d.datos.programado);
                                self.entregado(d.datos.entregado);
                                self.inicioCiclo(d.datos.inicioCiclo);
                                $.ajax({
                                    url: "Materiales/ObtenerMateriales", // URL del método GetAll en tu API
                                    type: "GET",
                                    contentType: "application/json",
                                    success: function (d) {
                                        self.registros.removeAll();
                                        self.registros.push.apply(self.registros, d.datos);
                                        $("#divEdicion").modal("hide");
                                        $("#alertModal").modal("show");
                                        
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
                        console.error("Error al actualizar el comentario: ", error);
                        alert("Error al actualizar el comentario: " + xhr.responseText);
                    }
                });
            },
            error: function (xhr, status, error) {
                console.error("Error al actualizar el comentario: ", error);
                alert("Error al actualizar el comentario: " + xhr.responseText);
            }
        });
    }
    self.buscar = function () {
        // Obtén los valores de los filtros
        var nombreProyecto = self.filtroNombreProyecto();
        var fechaEntrega = self.filtroFechaEntrega();

        // Construye la URL con parámetros
        var filtro = "";
        if (nombreProyecto) {
            filtro += "nombreProyecto=" + encodeURIComponent(nombreProyecto) + "&";
        }
        if (fechaEntrega) {
            filtro += "fechaEntrega=" + encodeURIComponent(fechaEntrega) + "&";
        }
        // Quita el último "&" si existe
        filtro = filtro.slice(0, -1);

        var material = {
            Nombre: nombreProyecto,
            FechaEntrega: fechaEntrega
        }
        // Llama al método ObtenerMateriales con los filtros aplicados
        $.ajax({
            url: "Materiales/ObtenerMaterialesPorNombre", // URL con filtros en query params
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(material),
            success: function (d) {
                // Actualiza los registros con los resultados obtenidos
                self.registros.removeAll();
                self.registros.push.apply(self.registros, d.datos);
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
    }
    // Método de búsqueda de usuarios con Autocompletar
    self.buscarUsuarios = function () {
        if (self.buscarUsuario().length < 3) {
            self.resultadosBusqueda([]); // Limpia resultados si menos de 3 caracteres
            return;
        }
        var usuario = {
            Nombre: self.buscarUsuario(),
        }

        $.ajax({
            url: "Usuarios/BuscarAllUsuarios", // Ruta de tu API para buscar usuarios
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(usuario),
            success: function (d) {
                self.resultadosBusqueda(d.datos); // Asigna resultados al array
            },
            error: function (xhr, status, error) {
                console.error("Error al buscar usuarios: ", error);
            }
        });
    };

    // Seleccionar usuario y agregar a Participante
    self.seleccionarUsuario = function (usuario) {

        self.registrosUsuariosCorreo.push(usuario);
        self.buscarUsuario(""); // Limpia el campo de búsqueda
        self.resultadosBusqueda([]); // Limpia resultados de autocompletado
        

    };
}
function setFiltroFromQueryString(viewModel) {
    const params = new URLSearchParams(window.location.search);
    const filtro = params.get("filtroNombre"); // Nombre del parámetro en el query string
    if (filtro) {
        viewModel.filtroNombre(filtro); // Asigna el valor al filtro
    }
}
// Activar Knockout.js
var appViewModel = new AppViewModel();
ko.applyBindings(appViewModel);
setFiltroFromQueryString(appViewModel);
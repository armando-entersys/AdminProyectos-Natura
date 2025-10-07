var estatusActivo = { IdEstatus: true, Estatus: "Activo" };
var estatusInactivo = { IdEstatus: false, Estatus: "Inactivo" };

var catEstatus = [estatusActivo, estatusInactivo];

function AppViewModel() {
    var self = this;

    // Observables principales
    self.registros = ko.observableArray([]);
    self.registrosHistorico = ko.observableArray([]);

    self.catEstatusMateriales = ko.observableArray([]);
    self.EstatusMateriales = ko.observable();

    self.catEstatusMaterialesFiltro = ko.observableArray([]);
    self.EstatusMaterialesFiltro = ko.observable();

    self.tituloModal = ko.observable("");

    self.revision = ko.observable(0);
    self.produccion = ko.observable(0);
    self.faltaInfo = ko.observable(0);
    self.aprobado = ko.observable(0);
    self.programado = ko.observable(0);
    self.entregado = ko.observable(0);
    self.inicioCiclo = ko.observable(0);
    self.noCompartio = ko.observable(0);
    self.linksReferencias = ko.observable("");
    self.idBrief = ko.observable(0);
    self.rutaArchivo = ko.observable("");

    // Observables para filtros
    self.filtroNombre = ko.observable("");
    self.filtroNombreProyecto = ko.observable("");
    self.filtroArea = ko.observable("");
    self.filtroResponsable = ko.observable("");
    self.filtroFechaEntrega = ko.observable("");
    self.filtroFechaInicio = ko.observable("");
    self.filtroFechaFin = ko.observable("");

    // Fecha mínima para selección
    const today = new Date();
    self.minDate = today.toISOString().split('T')[0];

    // Otros observables
    self.envioCorreo = ko.observable("No");
    self.id = ko.observable();
    self.Comentario = ko.observable("");
    self.fechaEntrega = ko.observable("");

    // Autocompletado
    self.buscarUsuario = ko.observable("");
    self.resultadosBusqueda = ko.observableArray([]);
    self.registrosUsuariosCorreo = ko.observableArray([]);

    // Paginación
    self.pageSize = ko.observable(8);
    self.currentPage = ko.observable(1);
    self.totalRegistros = ko.observable(0);

    // Función para verificar si el rol actual está permitido
    self.isRoleVisible = function (allowedRoles) {
        return allowedRoles.includes(RolId); // Asegúrate de que `RolId` esté definido en tu código
    };

    // Custom binding handler para evitar el reset del cursor en áreas de texto
    ko.bindingHandlers.textareaCursor = {
        update: function (element, valueAccessor) {
            var value = ko.unwrap(valueAccessor());
            var cursorPosition = element.selectionStart;
            element.value = value;
            setTimeout(function () {
                element.setSelectionRange(cursorPosition, cursorPosition);
            }, 0);
        }
    };

    // Computados
    self.registrosFiltrados = ko.computed(function () {
        var filtroNombreProyecto = self.filtroNombreProyecto().toLowerCase();
        var filtroNombre = self.filtroNombre().toLowerCase();
        var filtroArea = self.filtroArea().toLowerCase();
        var filtroResponsable = self.filtroResponsable().toLowerCase();
        var filtroEstatus = self.EstatusMaterialesFiltro(); // Obtener el estatus seleccionado

        return ko.utils.arrayFilter(self.registros(), function (registro) {
            var nombreProyecto = (registro.brief?.nombre || "").toLowerCase();
            var nombre = (registro.nombre || "").toLowerCase();
            var area = (registro.area || "").toLowerCase();
            var responsable = (registro.responsable || "").toLowerCase();
            var fechaEntrega = new Date(registro.fechaEntrega);

            var cumpleFiltroNombre = !filtroNombre || nombre.includes(filtroNombre);
            var cumpleFiltroProyecto = !filtroNombreProyecto || nombreProyecto.includes(filtroNombreProyecto);
            var cumpleFiltroArea = !filtroArea || area.includes(filtroArea);
            var cumpleFiltroResponsable = !filtroResponsable || responsable.includes(filtroResponsable);
            // Verifica el estatus
            var cumpleFiltroEstatus = !filtroEstatus || registro.estatusMaterialId === filtroEstatus.id;

            let cumpleFechas = true;
            if (self.filtroFechaInicio()) {
                cumpleFechas = cumpleFechas && fechaEntrega >= new Date(self.filtroFechaInicio());
            }
            if (self.filtroFechaFin()) {
                cumpleFechas = cumpleFechas && fechaEntrega <= new Date(self.filtroFechaFin());
            }

            return cumpleFiltroNombre && cumpleFiltroProyecto && cumpleFiltroArea && cumpleFiltroResponsable && cumpleFiltroEstatus && cumpleFechas;
        });
    });

    self.paginatedRegistros = ko.computed(function () {
        var startIndex = (self.currentPage() - 1) * self.pageSize();
        return self.registrosFiltrados().slice(startIndex, startIndex + self.pageSize());
    });

    self.totalPages = ko.computed(function () {
        return Math.ceil(self.registrosFiltrados().length / self.pageSize());
    });

    // Cambiar página
    self.goToPage = function (page) {
        if (page >= 1 && page <= self.totalPages()) {
            self.currentPage(page);
        }
    };

    // Inicializar datos
    self.inicializar = function () {
        $.get("Materiales/ObtenerMateriales")
            .then(function (d) {
                console.log("Datos de materiales:", d.datos); // Verifica la estructura de datos
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
                self.catEstatusMaterialesFiltro(d.datos);
            })
 
            .catch(function (error) {
                console.error("Error al inicializar los datos:", error);
            });
    };

    // Editar material
    self.Editar = function (material) {
        self.tituloModal("Editar Material: " + material.nombre);
        self.Comentario("");

        self.id(material.id);
        self.fechaEntrega(new Date(material.fechaEntrega).toISOString().split('T')[0]);
        self.registrosUsuariosCorreo.removeAll();
        self.idBrief(material.brief.id);
        if (material.brief.linksReferencias == "undefined") {
            self.linksReferencias("");
        }
        else {
            self.linksReferencias(material.brief.linksReferencias);
        }

        self.rutaArchivo(material.brief.rutaArchivo);
        var EstatusMateriales = self.catEstatusMateriales().find(function (r) {
            return r.id === material.estatusMaterialId;
        });
        self.EstatusMateriales(EstatusMateriales);

        $.get("Materiales/ObtenerHistorial/" + material.id)
            .then(function (d) {
                self.registrosHistorico(d.datos);
                $("#divEdicion").modal("show");

                // Esperar a que el modal esté completamente abierto y luego limpiar TinyMCE
                $('#divEdicion').one('shown.bs.modal', function () {
                    setTimeout(function() {
                        $('.tox-statusbar__branding').hide();
                        if (tinymce.get('myComentario')) {
                            tinymce.get('myComentario').setContent('');
                            tinymce.get('myComentario').focus();
                        }
                    }, 100);
                });
            })
            .catch(function (error) {
                console.error("Error al obtener el historial:", error);
            });
    };

    // Guardar comentario
    self.GuardarComentario = function () {
        // Obtener el valor del editor TinyMCE y establecerlo en el observable `Comentario`
        var comentarioContenido = tinymce.get('myComentario').getContent();
        self.Comentario(comentarioContenido);

        // Verificar si todos los campos requeridos están presentes
        if (!self.id() || !self.Comentario() || !self.EstatusMateriales() || !self.fechaEntrega()) {
            alert("Por favor, completa todos los campos requeridos antes de guardar.");
            return;
        }

        // Construir el objeto `HistorialMaterial`
        var historialMaterial = {
            MaterialId: self.id(),
            Comentarios: self.Comentario(),
            FechaEntrega: self.fechaEntrega(),
            EstatusMaterialId: self.EstatusMateriales().id
        };

        // Construir la solicitud completa
        var historialMaterialRequest = {
            HistorialMaterial: historialMaterial,
            EnvioCorreo: self.envioCorreo() !== "No",
            Usuarios: self.registrosUsuariosCorreo()
        };

        // Cerrar el modal antes de ejecutar la solicitud
        $("#divEdicion").modal("hide");

        // Iniciar el loader manualmente (por si ajaxSetup no lo detecta)
        $('#loader-overlay').removeClass('d-none');

        // Enviar la solicitud
        $.ajax({
            url: "Materiales/AgregarHistorialMaterial",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(historialMaterialRequest),
            success: function (response) {
                // Mostrar mensaje de éxito y refrescar los datos
                showAlertModal(response.mensaje);
                
                self.inicializar();
            },
            error: function (xhr, status, error) {
                console.error("Error al guardar el comentario:", xhr.responseText || error);
                alert("Error al guardar el comentario. Revisa los datos y vuelve a intentar.");
            },
            complete: function () {
                // Ocultar el loader cuando la solicitud termina
                $('#loader-overlay').addClass('d-none');
            }
        });
    };



    // Buscar usuarios con autocompletar
    self.buscarUsuarios = function () {
        if (self.buscarUsuario().length < 3) {
            self.resultadosBusqueda([]); // Limpia resultados si menos de 3 caracteres
            return;
        }

        // Construir el objeto para enviar
        var usuarioRequest = {
            Nombre: self.buscarUsuario()
        };

        // Enviar la solicitud AJAX
        $.ajax({
            url: "Usuarios/BuscarAllUsuarios",
            type: "POST",
            contentType: "application/json", // Asegúrate de que el Content-Type sea JSON
            data: JSON.stringify(usuarioRequest), // Convertir el objeto a JSON
            success: function (response) {
                self.resultadosBusqueda(response.datos); // Actualizar los resultados con los datos obtenidos
            },
            error: function (xhr, status, error) {
                console.error("Error al buscar usuarios:", xhr.responseText || error);
                alert("Error al buscar usuarios. Revisa los datos y vuelve a intentar.");
            }
        });
    };


    // Seleccionar usuario
    self.seleccionarUsuario = function (usuario) {
        self.registrosUsuariosCorreo.push(usuario);
        self.buscarUsuario("");
        self.resultadosBusqueda([]);
    };

    // Método para setear filtros desde el QueryString
    self.setFiltroFromQueryString = function () {
        const params = new URLSearchParams(window.location.search);
        const filtro = params.get("filtroNombre");
        if (filtro) {
            self.filtroNombre(filtro);
        }
    };
    self.exportarExcel = function () {
        // Obtener los datos filtrados
        var data = self.registrosFiltrados().map(function (registro) {
            return {
                "Nombre de Material": registro.nombre || "",
                "Mensaje": registro.mensaje || "",
                "Formato": registro.formato?.descripcion || "",
                "Estatus": registro.estatusMaterial?.descripcion || "",
                "Nombre del Proyecto": registro.brief?.nombre || "",
                "Audiencia": registro.audiencia?.descripcion || "",
                "Responsable": registro.responsable || "",
                "Área": registro.area || "",
                "Fecha de Entrega": registro.fechaEntrega || ""
            };
        });

        // Verificar si hay datos para exportar
        if (data.length === 0) {
            alert("No hay datos para exportar.");
            return;
        }

        // Crear una hoja de trabajo
        var worksheet = XLSX.utils.json_to_sheet(data);

        // Crear un libro de trabajo
        var workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Materiales Filtrados");

        // Generar el archivo Excel y descargarlo
        XLSX.writeFile(workbook, "MaterialesFiltrados.xlsx");
    };

    // Inicializar al cargar
    self.inicializar();
}

// Activar Knockout.js
var appViewModel = new AppViewModel();
ko.applyBindings(appViewModel);
appViewModel.setFiltroFromQueryString();

    var estatusActivo = { IdEstatus: true, Estatus: "Activo" };
var estatusInactivo = { IdEstatus: false, Estatus: "Inactivo" };

var catEstatus = [estatusActivo, estatusInactivo];
function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray();
    self.catEstatusMateriales = ko.observableArray();
    self.EstatusMateriales = ko.observable();

    self.revision = ko.observable();
    self.produccion = ko.observable();
    self.faltaInfo = ko.observable();
    self.programado = ko.observable();
    self.entregado = ko.observable();
    self.inicioCiclo = ko.observable();
    self.noCompartio = ko.observable();
    // Observables para filtros
    self.filtroNombreProyecto = ko.observable("");
    self.filtroFechaEntrega = ko.observable("");

    self.id = ko.observable();
    self.Comentario = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });

    self.fechaEntrega = ko.observable();
    // Custom binding handler to prevent cursor reset
    ko.bindingHandlers.textareaCursor = {
        init: function (element, valueAccessor) {
            let observable = valueAccessor();

            // Actualiza el observable cuando el usuario escribe
            element.addEventListener('input', function () {
                observable(element.value);
            });
        },
        update: function (element, valueAccessor) {
            let value = ko.unwrap(valueAccessor());

            // Guarda la posición actual del cursor
            const cursorPos = element.selectionStart;

            // Solo actualiza si el valor ha cambiado
            if (element.value !== value) {
                element.value = value;

                // Restaura la posición del cursor
                element.setSelectionRange(cursorPos, cursorPos);
            }
        }
    };
    ko.bindingHandlers.stableTextInput = {
        init: function (element, valueAccessor) {
            // Obtener el valor observable
            let observable = valueAccessor();

            // Escuchar cambios en el input del usuario
            element.addEventListener('input', function () {
                observable(element.value);
            });

            // Monitorear cambios externos y mantener la posición del cursor
            ko.utils.registerEventHandler(element, "blur", function () {
                let value = ko.unwrap(observable);

                // Solo actualizar si el valor cambia
                if (element.value !== value) {
                    element.value = value;
                }
            });
        },
        update: function (element, valueAccessor) {
            let value = ko.unwrap(valueAccessor());

            // Actualizar solo si el valor es diferente
            if (element.value !== value) {
                // Guardar la posición actual del cursor
                const cursorPos = element.selectionStart;

                // Actualizar el valor sin modificar el cursor
                element.value = value;

                // Restaurar la posición del cursor
                element.setSelectionRange(cursorPos, cursorPos);
            }
        }
    };
    self.inicializar = function () {
        $.ajax({
            url: "/Materiales/ObtenerMateriales", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registros.removeAll();
                self.registros.push.apply(self.registros, d.datos.$values);
                $.ajax({
                    url: "/Materiales/ObtenerConteoEstatusMateriales", // URL del método GetAll en tu API
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        self.revision(d.datos.revision);
                        self.produccion(d.datos.produccion);
                        self.faltaInfo(d.datos.faltaInfo);
                        self.programado(d.datos.programado);
                        self.entregado(d.datos.entregado);
                        self.inicioCiclo(d.datos.inicioCiclo);
                        self.noCompartio(d.datos.noCompartio);
                        $.ajax({
                            url: "/Materiales/ObtenerEstatusMateriales", // URL del método GetAll en tu API
                            type: "GET",
                            contentType: "application/json",
                            success: function (d) {
                                self.catEstatusMateriales.removeAll();
                                self.catEstatusMateriales.push.apply(self.catEstatusMateriales, d.datos.$values);
                                // Inicializar CKEditor después de que el DOM esté listo
                                // Inicializar CKEditor después de que el DOM esté listo y los datos se hayan cargado
                                setTimeout(() => {
                                    ClassicEditor
                                        .create(document.querySelector('#comentario-editor'), {
                                            toolbar: [
                                                'heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote', '|', 'undo', 'redo', '|', 'imageUpload'
                                            ],
                                            simpleUpload: {
                                                uploadUrl: '/tu-servidor-de-carga', // URL para manejar la carga de imágenes
                                                headers: {
                                                    'X-CSRF-TOKEN': 'tu-csrf-token'
                                                }
                                            }
                                        })
                                        .then(editor => {
                                            // Sincronizar el contenido de CKEditor con el observable de Knockout
                                            editor.model.document.on('change:data', () => {
                                                self.Comentario(editor.getData());
                                            });

                                            // Escuchar cambios del observable y actualizar CKEditor
                                            self.Comentario.subscribe(newValue => {
                                                editor.setData(newValue);
                                            });
                                        })
                                        .catch(error => {
                                            console.error("Error al inicializar CKEditor: ", error);
                                        });
                                }, 100);

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
           
    }
    self.inicializar();
    function updateComentario(textarea) {
        var viewModel = ko.dataFor(textarea);
        viewModel.Comentario(textarea.value);
    }
    // Computed observable para filtrar registros
    self.registrosFiltrados = ko.computed(function () {
        var filtroNombre = self.filtroNombreProyecto().toLowerCase();
        var filtroFecha = self.filtroFechaEntrega();

        return ko.utils.arrayFilter(self.registros(), function (registro) {
            var nombreProyecto = registro.brief.nombre.toLowerCase();
            var fechaEntrega = registro.fechaEntrega;

            // Filtros: por nombre de proyecto y por fecha de entrega
            var cumpleNombre = filtroNombre === "" || nombreProyecto.includes(filtroNombre);
            var cumpleFecha = filtroFecha === "" || fechaEntrega === filtroFecha;

            return cumpleNombre && cumpleFecha;
        });
    });

    self.Editar = function (material) {
        self.id(material.id);
        self.fechaEntrega(material.fechaEntrega);
        var EstatusMateriales = self.catEstatusMateriales().find(function (r) {
            return r.id === material.estatusMaterialesId;
        });
        self.EstatusMateriales(EstatusMateriales);

        $("#divEdicion").modal("show");
    }
    self.GuardarComentario = function () {
        // Obtener el valor del editor y establecerlo en el observable `Comentario`
        var comentarioHTML = CKEDITOR.instances['comentario-editor'].getData();
        self.Comentario(comentarioHTML);

        var historialMaterial = {
            MaterialId: self.id(),
            Comentarios: self.Comentario(),
            FechaRegistro: self.fechaEntrega()
        }

        $.ajax({
            url: "/Materiales/ActualizarMaterial",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(historialMaterial),
            success: function (d) {
                alert("Comentario actualizado correctamente");
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
            url: "/Materiales/ObtenerMaterialesPorNombre", // URL con filtros en query params
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(material),
            success: function (d) {
                // Actualiza los registros con los resultados obtenidos
                self.registros.removeAll();
                self.registros.push.apply(self.registros, d.datos.$values);
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
    }

}

// Activar Knockout.js
ko.applyBindings(new AppViewModel());
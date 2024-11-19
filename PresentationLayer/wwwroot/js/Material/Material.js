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
    self.filtroNombreProyecto = ko.observable("");
    self.filtroFechaEntrega = ko.observable("");
    self.envioCorreo = ko.observable("No");
    self.id = ko.observable();
    self.Comentario = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });

    self.fechaEntrega = ko.observable();
    // Observables y variables para autocompletado
    self.buscarUsuario = ko.observable("");
    self.resultadosBusqueda = ko.observableArray([]);
    self.registrosUsuariosCorreo = ko.observableArray();

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
    let comentarioEditor;
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
                        self.aprobado(d.datos.aprobado);
                        self.programado(d.datos.programado);
                        self.entregado(d.datos.entregado);
                        self.inicioCiclo(d.datos.inicioCiclo);
                        
                        $.ajax({
                            url: "/Materiales/ObtenerEstatusMateriales", // URL del método GetAll en tu API
                            type: "GET",
                            contentType: "application/json",
                            success: function (d) {
                                self.catEstatusMateriales.removeAll();
                                self.catEstatusMateriales.push.apply(self.catEstatusMateriales, d.datos.$values);

                                
                                ClassicEditor
                                    .create(document.querySelector('#comentario-editor'), {
                                        toolbar: [
                                            'heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote', '|', 'undo', 'redo', '|', 'imageUpload'
                                        ],
                                        simpleUpload: {
                                            uploadUrl: '/Materiales/UploadImage',
                                            headers: {
                                                'X-CSRF-TOKEN': 'tu-csrf-token'
                                            }
                                        }
                                    })
                                    .then(editor => {
                                        comentarioEditor = editor; // Asigna el editor a la variable global
                                        editor.model.document.on('change:data', () => {
                                            self.Comentario(editor.getData());
                                        });
                                        self.Comentario.subscribe(newValue => {
                                            editor.setData(newValue);
                                        });
                                    })
                                    .catch(error => {
                                        console.error("Error al inicializar CKEditor: ", error);
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
            var nombreProyecto = registro.brief.nombre;
            var fechaEntrega = registro.fechaEntrega;

            // Filtros: por nombre de proyecto y por fecha de entrega
            var cumpleNombre = filtroNombre === "" || nombreProyecto.includes(filtroNombre);
            var cumpleFecha = filtroFecha === "" || fechaEntrega === filtroFecha;

            return cumpleNombre && cumpleFecha;
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
            url: "/Materiales/ObtenerHistorial/" + material.id,
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registrosHistorico.removeAll();
                self.registrosHistorico.push.apply(self.registrosHistorico, d.datos.$values);
                
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
            url: "/Materiales/AgregarHistorialMaterial", 
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(historialMaterialRequest),
            success: function (d) {
               
                $.ajax({
                    url: "/Materiales/ObtenerHistorial/" + self.id(),
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        alert("Comentario actualizado correctamente");
                        self.registrosHistorico.removeAll();
                        self.registrosHistorico.push.apply(self.registrosHistorico, d.datos.$values);
                        $.ajax({
                            url: "/Materiales/ObtenerConteoEstatusMateriales", // URL del método GetAll en tu API
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
            url: "/Usuarios/BuscarAllUsuarios", // Ruta de tu API para buscar usuarios
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(usuario),
            success: function (d) {
                self.resultadosBusqueda(d.datos.$values); // Asigna resultados al array
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

// Activar Knockout.js
ko.applyBindings(new AppViewModel());
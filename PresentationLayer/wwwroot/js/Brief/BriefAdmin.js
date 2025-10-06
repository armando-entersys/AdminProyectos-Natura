
function Task(id, title, usuarioId, nombreUsuario, fechaEntrega) {
    this.id = id;
    this.title = title;
    this.usuarioId = usuarioId;
    this.nombreUsuario = nombreUsuario;
    this.fechaEntrega = fechaEntrega;
}

// Define el ViewModel de la columna
function Column(id, name, tasks) {
    this.id = id;
    this.name = name;
    this.tasks = ko.observableArray(tasks); // Tareas de la columna
    this.searchTitle = ko.observable(""); // Campo de búsqueda por título

    // Filtrar tareas basado en el título
    this.filteredTasks = ko.computed(function () {
        var search = this.searchTitle().toLowerCase(); // Obtener el valor de búsqueda
        return this.tasks().filter(function (task) {
            return task.title.toLowerCase().includes(search); // Filtrar tareas por título
        });
    }, this);
}

function AppViewModel() {
    var self = this;

    self.columns = ko.observableArray();
    self.columns2 = ko.observableArray();

    self.id = ko.observable(0);
    self.nombre = ko.observable();
    self.descripcion = ko.observable();
    self.objetivo = ko.observable();
    self.dirigidoA = ko.observable();
    self.comentario = ko.observable();
    self.rutaArchivo = ko.observable();
    self.linksReferencias = ko.observable();

    self.catEstatusBrief = ko.observableArray();
    self.EstatusBrief = ko.observable();
    self.catTipoBrief = ko.observableArray();
    self.TipoBrief = ko.observable();
    
    self.cargaArchivo = ko.observable();
    self.registros = ko.observableArray();

    self.determinarEstado = ko.observable();
    self.fechaPublicacion = ko.observable();

    self.nombreMaterial = ValidationModule.validations.requiredField();
    self.mensaje = ValidationModule.validations.requiredField();
    self.catPrioridad = ko.observableArray();
    self.prioridad = ValidationModule.validations.requiredField();
    self.ciclo = ValidationModule.validations.requiredField();
    self.catPCN = ko.observableArray();
    self.pcn = ValidationModule.validations.requiredField();
    self.formato = ValidationModule.validations.requiredField();
    self.catFormato = ko.observableArray();
    self.audiencia = ValidationModule.validations.requiredField();
    self.catAudiencia = ko.observableArray();

    self.fechaEntrega = ValidationModule.validations.requiredField();
    self.responsable = ValidationModule.validations.requiredField();
    self.area = ValidationModule.validations.requiredField();


    self.determinarEstado = ValidationModule.validations.requiredField();
    self.planComunicacion = ValidationModule.validations.requiredField();
    self.fechaPublicacion = ValidationModule.validations.requiredField();
    self.comentarioProyecto = ko.observable("");

    self.registrosMateriales = ko.observableArray();
    self.registrosParticipantes = ko.observableArray();

    // Observables y variables para autocompletado
    self.buscarUsuario = ko.observable("");
    self.resultadosBusqueda = ko.observableArray([]);

    // Obtener la fecha actual en formato YYYY-MM-DD
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0'); // Mes en formato 2 dígitos
    const day = String(today.getDate()).padStart(2, '0'); // Día en formato 2 dígitos

    // Formato de fecha mínima
    self.minDate = `${year}-${month}-${day}`; // YYYY-MM-DD

    self.errors = ko.validation.group(self);
    self.inicializar = function () {
        $.ajax({
            url: "Brief/GetAllColumns", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.columns.removeAll();
                // Asegúrate de que los datos se transformen en instancias de Task y Column
                var transformedColumns = d.datos.map(function (columnData) {
                    var tasks = columnData.tasks.map(function (taskData) {
                        return new Task(taskData.id, taskData.title, taskData.usuarioId, taskData.nombreUsuario, taskData.fechaEntrega);
                    });
                    return new Column(columnData.id, columnData.name, tasks);
                });

                self.columns.push.apply(self.columns, transformedColumns); // Añadimos las columnas transformadas

                // Inicializa SortableJS una vez que los datos han sido cargados y Knockout haya renderizado
                setTimeout(function() {
                    initializeSortable();
                }, 100);
                $.ajax({
                    url: "Brief/GetAllEstatusBrief", // URL del método GetAll en tu API
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        self.catEstatusBrief.removeAll();
                        self.catEstatusBrief.push.apply(self.catEstatusBrief, d.datos);
                        $("#divEdicion").modal("hide");
                        $.ajax({
                            url: "Brief/GetAllTipoBrief", // URL del método GetAll en tu API
                            type: "GET",
                            contentType: "application/json",
                            success: function (d) {
                                self.catTipoBrief.removeAll();
                                self.catTipoBrief.push.apply(self.catTipoBrief, d.datos);

                                $.ajax({
                                    url: "Brief/GetAllPrioridad",
                                    type: "GET",
                                    contentType: "application/json",
                                    success: function (d) {
                                        self.catPrioridad.removeAll();
                                        self.catPrioridad.push.apply(self.catPrioridad, d.datos);
                                        $.ajax({
                                            url: "Brief/GetAllPCN",
                                            type: "GET",
                                            contentType: "application/json",
                                            success: function (d) {
                                                self.catPCN.removeAll();
                                                self.catPCN.push.apply(self.catPCN, d.datos);
                                                $.ajax({
                                                    url: "Brief/GetAllFormatos",
                                                    type: "GET",
                                                    contentType: "application/json",
                                                    success: function (d) {
                                                        self.catFormato.removeAll();
                                                        self.catFormato.push.apply(self.catFormato, d.datos);
                                                        $.ajax({
                                                            url: "Brief/GetAllAudiencias",
                                                            type: "GET",
                                                            contentType: "application/json",
                                                            success: function (d) {
                                                                self.catAudiencia.removeAll();
                                                                self.catAudiencia.push.apply(self.catAudiencia, d.datos);
                                                                $("#divEdicion").modal("hide");
                                                                const params = new URLSearchParams(window.location.search);
                                                                const filtro = params.get("filtroNombre");
                                                                if (filtro) {
                                                                    self.columns().forEach(function (element, index, array) {
                                                                        element.searchTitle(filtro)
                                                                    });
                                                                }
                                                               
                                                            },
                                                            error: function (xhr, status, error) {
                                                                console.error("Error al obtener los datos: ", xhr.responseText);
                                                                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                                                                $('#alertModalLabel').text("Error");
                                                                $("#alertModal").modal("show");
                                                            }
                                                        });
                                                    },
                                                    error: function (xhr, status, error) {
                                                        console.error("Error al obtener los datos: ", xhr.responseText);
                                                        $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                                                        $('#alertModalLabel').text("Error");
                                                        $("#alertModal").modal("show");
                                                    }
                                                });
                                            },
                                            error: function (xhr, status, error) {
                                                console.error("Error al obtener los datos: ", xhr.responseText);
                                                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                                                $('#alertModalLabel').text("Error");
                                                $("#alertModal").modal("show");
                                            }
                                        });

                                    },
                                    error: function (xhr, status, error) {
                                        console.error("Error al obtener los datos: ", xhr.responseText);
                                        $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                                        $('#alertModalLabel').text("Error");
                                        $("#alertModal").modal("show");
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
    self.Editar = function (brief) {
        self.Limpiar();
        self.LimpiarMaterial();
        self.id(brief.id);
        $.ajax({
            url: "Brief/Details/" + self.id(), // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.nombre(d.datos.nombre);
                self.descripcion(d.datos.descripcion);
                self.objetivo(d.datos.objetivo);
                self.dirigidoA(d.datos.dirigidoA);
                self.comentario(d.datos.comentario);
                self.rutaArchivo(d.datos.rutaArchivo);
                self.fechaEntrega(new Date(d.datos.fechaEntrega).toISOString().split('T')[0]);
                self.linksReferencias(d.datos.linksReferencias);
                var EstatusBrief = self.catEstatusBrief().find(function (r) {
                    return r.id === d.datos.estatusBriefId;
                });
                self.EstatusBrief(EstatusBrief);
                var TipoBrief = self.catTipoBrief().find(function (r) {
                    return r.id === d.datos.tipoBriefId;
                });
                self.TipoBrief(TipoBrief);
                self.ObtenerProyecto(self.id());
                self.ObtenerMateriales(self.id());
                $.ajax({
                    url: "Usuarios/ObtenerParticipantes/" + d.datos.id, // URL del método GetAll en tu API
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        self.registrosParticipantes.removeAll();
                        self.registrosParticipantes.push.apply(self.registrosParticipantes, d.datos);
                        $("#divEdicion").modal("show");

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
    self.EliminarBrief = function (brief) {
        if (confirm("Desea eliminar el Brief seleccionado?")) {
            $.ajax({
                url: "Brief/EliminarBrief/" + self.id(), // URL del método GetAll en tu API
                type: "GET",
                contentType: "application/json",
                success: function (d) {
                    self.inicializar();
                },
                error: function (xhr, status, error) {
                    console.error("Error al obtener los datos: ", error);
                    alert("Error al obtener los datos: " + xhr.responseText);
                }
            });
        }
        

    }
    
    // Método para comprobar si el rol actual coincide con el pasado
    self.isRoleVisible = function (allowedRoles) {
        return allowedRoles.includes(RolId);
    };
    self.addTask = function (column) {
        var newId = column.tasks().length + 1;
        column.tasks.push(new Task(newId, 'New Task ' + newId));
    };

    self.clearTasks = function (column) {
        column.tasks.removeAll(); // Limpia todas las tareas de la columna
    };

    self.refreshTasks = function (column, taskToMove, targetColumn) {
        column.tasks.remove(taskToMove);
        targetColumn.tasks.push(taskToMove);
        column.tasks.valueHasMutated();  // Actualizar columna origen
        targetColumn.tasks.valueHasMutated();  // Actualizar columna destino
    };

    self.moveTask = function (taskId, fromColumnId, toColumnId, newIndex) {
        var brief = {
            Id: taskId,
            EstatusBriefId: toColumnId
        }
        $.ajax({
            url: "Brief/EditStatus", // URL del método GetAll en tu API
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify(brief),
            success: function (d) {
                self.inicializar();
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });

    };
    self.cargarArchivo = function (data, event) {
        var file = event.target.files[0];
        if (file) {
            var fileType = file.type;
            var validTypes = ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];

            if (validTypes.includes(fileType)) {
                // Aquí podrías procesar el archivo si lo necesitas
                self.cargaArchivo(file); // Guardar el archivo seleccionado
            } else {
                alert('Solo se permiten archivos PDF o DOCX');
                event.target.value = "";  // Limpiar el campo de archivo
            }
        }
    };
    self.Limpiar = function () {
        self.id(0);
        self.nombre("");
        self.descripcion("");
        self.objetivo("");
        self.dirigidoA("");
        self.comentario("");
        self.rutaArchivo("");
        self.fechaEntrega("");
        self.cargaArchivo("");
    }
    self.GuardarProyecto = function () {
        var PlanComunicacion = false;
        if (self.planComunicacion() === "Sí") {
            PlanComunicacion = true;
        }

        var proyecto = {
            BriefId: self.id(),
            EstatusBriefId: self.EstatusBrief() ? self.EstatusBrief().id : null,
            Comentario: self.comentarioProyecto(),
            Estado: self.determinarEstado(),
            RequierePlan: PlanComunicacion,
            FechaPublicacion: self.fechaPublicacion()
        };

        console.log("Datos del proyecto:", proyecto); // Añade esto para revisar en consola

        $.ajax({
            url: "Brief/CreateProyecto",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(proyecto),
            success: function (d) {
                $("#divEdicion").modal("hide");
                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Solicitud exitosa");
                $("#alertModal").modal("show");
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", xhr.responseText);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
       
    }
    self.LimpiarMaterial = function () {
        self.nombreMaterial("");
        self.mensaje("");
        self.ciclo("");
        self.responsable("");
        self.area("");
        self.prioridad("");
        self.pcn("");
        self.formato("");
        self.audiencia("");
    }
    self.GuardarMaterial = function () {
        
        validarYProcesarFormulario(self.errors, function () {
            var Material = {
                BriefId: self.id(),
                Nombre: self.nombreMaterial(),
                Mensaje: self.mensaje(),
                PrioridadId: self.prioridad().id,
                Ciclo: self.ciclo(),
                PCNId: self.pcn().id,
                AudienciaId: self.audiencia().id,
                FormatoId: self.formato().id,
                FechaEntrega: self.fechaEntrega(),
                Responsable: self.responsable(),
                Area: self.area()
            };

            $.ajax({
                url: "Brief/CreateMaterial", // URL del método en tu API
                type: "POST",
                contentType: "application/json", // Cambiado a JSON
                data: JSON.stringify(Material),  // Serializamos los datos a JSON
                success: function (d) {
                    
                    self.ObtenerMateriales(self.id());
                    self.LimpiarMaterial();
                    
                    alert(d.mensaje);

                },
                error: function (xhr, status, error) {
                    console.error("Error al obtener los datos: ", error);
                    $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                    $('#alertModalLabel').text("Error");
                    $("#alertModal").modal("show");
                }
            });
        });  
    }
    self.ObtenerProyecto = function (id) {
        $.ajax({
            url: "Brief/ObtenerProyectoPorBrief/" + id, // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                if (d.datos != undefined) {
                    var PlanComunicacion = "No";
                    if (d.datos.requierePlan) {
                        PlanComunicacion = "Sí";
                    }
                    self.planComunicacion(PlanComunicacion);
                    self.determinarEstado(d.datos.estado);
                    self.comentarioProyecto(d.datos.comentario);
                    self.fechaPublicacion(new Date(d.datos.fechaPublicacion).toISOString().split('T')[0]);
                }
                else {
                    self.planComunicacion("No");
                    self.comentarioProyecto("");
                    self.fechaPublicacion(new Date().toISOString().split('T')[0]);
                }

                $("#divEdicion").modal("show");
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });

    }
    self.ObtenerMateriales = function (id) {
        $.ajax({
            url: "Brief/ObtenerMateriales/" + id, // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registrosMateriales.removeAll();
                self.registrosMateriales.push.apply(self.registrosMateriales, d.datos);
                
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });

    }
    self.EliminarMaterial = function (material) {
        if (confirm("Desea eliminar el Material seleccionado?")) {
            $.ajax({
                url: "Brief/EliminarMaterial/" + material.id, // URL del método GetAll en tu API
                type: "GET",
                contentType: "application/json",
                success: function (d) {
                    self.ObtenerMateriales(self.id());
                },
                error: function (xhr, status, error) {
                    console.error("Error al obtener los datos: ", error);
                    alert("Error al obtener los datos: " + xhr.responseText);
                }
            });
        }
      

    }
    self.EliminarParticipante = function (participante) {
        if (confirm("Desea eliminar el Participante seleccionado?")) {
            $.ajax({
                url: "Brief/EliminarParticipante/" + participante.id, // URL del método GetAll en tu API
                type: "GET",
                contentType: "application/json",
                success: function (d) {
                  
                    $.ajax({
                        url: "Usuarios/ObtenerParticipantes/" + self.id(), // URL del método GetAll en tu API
                        type: "GET",
                        contentType: "application/json",
                        success: function (d) {
                            self.registrosParticipantes.removeAll();
                            self.registrosParticipantes.push.apply(self.registrosParticipantes, d.datos);
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
            url: "Usuarios/BuscarUsuario", // Ruta de tu API para buscar usuarios
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
  
        var participante = {
            BriefId: self.id(),
            UsuarioId : usuario.id
        }
        $.ajax({
            url: "Usuarios/AgregarParticipante", // Ruta de tu API para buscar usuarios
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(participante),
            success: function (d) {
                
                self.buscarUsuario(""); // Limpia el campo de búsqueda
                self.resultadosBusqueda([]); // Limpia resultados de autocompletado
                $.ajax({
                    url: "Usuarios/ObtenerParticipantes/" + self.id(), // URL del método GetAll en tu API
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        self.registrosParticipantes.removeAll();
                        self.registrosParticipantes.push.apply(self.registrosParticipantes, d.datos);
                        $("#divEdicion").modal("show");

                    },
                    error: function (xhr, status, error) {
                        console.error("Error al obtener los datos: ", error);
                        alert("Error al obtener los datos: " + xhr.responseText);
                    }
                });

            },
            error: function (xhr, status, error) {
                console.error("Error al buscar usuarios: ", error);
            }
        });
       
    };
    self.setFiltroFromQueryString = function () {
        const params = new URLSearchParams(window.location.search);
        const filtro = params.get("filtroNombre");
        if (filtro) {
            searchTitle(filtro);
        }
    };
}

// Inicializa SortableJS después de que Knockout haya sido inicializado
function initializeSortable() {
    document.querySelectorAll('.sortable').forEach(function (element) {
        new Sortable(element, {
            group: 'kanban',
            animation: 150,
            onEnd: function (evt) {
                var taskId = parseInt(evt.item.getAttribute('data-task-id'));
                var fromColumnId = parseInt(evt.from.id);
                var toColumnId = parseInt(evt.to.id);
                var newIndex = evt.newIndex;

                appViewModel.moveTask(taskId, fromColumnId, toColumnId, newIndex);
            }
        });
    });
}

var appViewModel = new AppViewModel();
ko.applyBindings(appViewModel);
//appViewModel.setFiltroFromQueryString();
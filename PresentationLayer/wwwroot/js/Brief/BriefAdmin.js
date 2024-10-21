
function Task(id, title, UsuarioId, NombreUsuario, FechaEntrega) {
    this.id = id;
    this.title = ko.observable(title);
    this.usuarioId = ko.observable(UsuarioId);
    this.nombreUsuario = ko.observable(NombreUsuario);
    this.fechaEntrega = ko.observable(FechaEntrega)
}

function Column(id, name, tasks) {
    this.id = id;
    this.name = ko.observable(name);
    this.tasks = ko.observableArray(tasks);
}
var PrioridadPequena = { Id: 1, Descripcion: "Pequeña" };
var PrioridadMediana = { Id: 2, Descripcion: "Mediana" };
var PrioridadGrande = { Id: 3, Descripcion: "Grande" };
var catPrioridad = [PrioridadPequena, PrioridadMediana, PrioridadGrande];

var catPCN = [{ Id: 1, Descripcion: "Medio" }
    , { Id: 2, Descripcion: "Facebook cocreando" }
    , { Id: 3, Descripcion: "Mi natura digital" }
    , { Id: 4, Descripcion: "Whatsapp" }
    , { Id: 5, Descripcion: "Mailing" }
    , { Id: 6, Descripcion: "Instagram" }
    , { Id: 7, Descripcion: "Folleto digital" }];

var catFormato = [{ Id: 1, Descripcion: "placa" }
    , { Id: 1, Descripcion: "panel home" }
    , { Id: 2, Descripcion: "panel captación" }
    , { Id: 3, Descripcion: "vitrina" }
    , { Id: 4, Descripcion: "history" }
    , { Id: 5, Descripcion: "video" }
    , { Id: 6, Descripcion: "card" }
    , { Id: 7, Descripcion: "comunicado" }
    , { Id: 8, Descripcion: "email " }
    , { Id: 9, Descripcion: "guia interactiva" }
    , { Id: 10, Descripcion: "pdf" }
    , { Id: 11, Descripcion: "infografia" }
    , { Id: 12, Descripcion: "landing page" }
    , { Id: 13, Descripcion: "placa animada" }
    , { Id: 14, Descripcion: "posteo" }
    , { Id: 15, Descripcion: "video tictoc " }
    , { Id: 16, Descripcion: "video" }];

var catAudiencia = [{ Id: 1, Descripcion: "Consultor" }
    , { Id: 2, Descripcion: "Cliente final" }
    , { Id: 3, Descripcion: "Líderes" }
    , { Id: 4, Descripcion: "Segmento especifico" }]

function AppViewModel() {
    var self = this;

    self.columns = ko.observableArray();
    self.columns2 = ko.observableArray();

    self.id = ko.observable(0);
    self.nombre = ko.observable().extend({ required: true });
    self.descripcion = ko.observable().extend({ required: true });
    self.objetivo = ko.observable().extend({ required: true });
    self.dirigidoA = ko.observable().extend({ required: true });
    self.comentario = ko.observable().extend({ required: true });
    self.rutaArchivo = ko.observable().extend({ required: true });
    self.fechaEntrega = ko.observable().extend({ required: true });

    self.catEstatusBrief = ko.observableArray();
    self.EstatusBrief = ko.observable().extend({ required: true });
    self.catTipoBrief = ko.observableArray();
    self.TipoBrief = ko.observable().extend({ required: true });
    self.catClasificacionProyecto = ko.observableArray();
    self.ClasificacionProyecto = ko.observable().extend({ required: true });
    
    self.cargaArchivo = ko.observable().extend({ required: true });
    self.registros = ko.observableArray();
    self.planComunicacion = ko.observable();

    self.determinarEstado = ko.observable();
    self.fechaPublicacion = ko.observable();
    self.nombreMaterial = ko.observable();

    self.mensaje = ko.observable();
    self.catPrioridad = ko.observableArray();
    self.prioridad = ko.observable();
    self.ciclo = ko.observable();
    self.catPCN = ko.observableArray();
    self.pnc = ko.observable();
    self.formato = ko.observable();
    self.catFormato = ko.observableArray();
    self.audiencia = ko.observable();
    self.catAudiencia = ko.observableArray();

    self.fechaEntrega = ko.observable();
    self.proceso = ko.observable();
    self.produccion = ko.observable();
    self.respondable = ko.observable();

    self.determinarEstado = ko.observable();
    self.planComunicacion = ko.observable();
    self.fechaPublicacion = ko.observable();
    self.comentarioProyecto = ko.observable();
   
    self.inicializar = function () {
        $.ajax({
            url: "/Brief/GetAllColumns", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.columns.removeAll();
                // Asegúrate de que los datos se transformen en instancias de Task y Column
                var transformedColumns = d.datos.$values.map(function (columnData) {
                    var tasks = columnData.tasks.$values.map(function (taskData) {
                        return new Task(taskData.id, taskData.title, taskData.usuarioId, taskData.nombreUsuario, taskData.fechaEntrega);
                    });
                    return new Column(columnData.id, columnData.name, tasks);
                });

                self.columns.push.apply(self.columns, transformedColumns); // Añadimos las columnas transformadas

                // Inicializa SortableJS una vez que los datos han sido cargados
                initializeSortable();
                $.ajax({
                    url: "/Brief/GetAllEstatusBrief", // URL del método GetAll en tu API
                    type: "GET",
                    contentType: "application/json",
                    success: function (d) {
                        self.catEstatusBrief.removeAll();
                        self.catEstatusBrief.push.apply(self.catEstatusBrief, d.datos.$values);
                        $("#divEdicion").modal("hide");
                        $.ajax({
                            url: "/Brief/GetAllTipoBrief", // URL del método GetAll en tu API
                            type: "GET",
                            contentType: "application/json",
                            success: function (d) {
                                self.catTipoBrief.removeAll();
                                self.catTipoBrief.push.apply(self.catTipoBrief, d.datos.$values);

                                self.catPrioridad.removeAll();
                                self.catPrioridad.push.apply(self.catPrioridad, catPrioridad);

                                self.catPCN.removeAll();
                                self.catPCN.push.apply(self.catPCN, catPCN);

                                self.catFormato.removeAll();
                                self.catFormato.push.apply(self.catFormato, catFormato);

                                self.catAudiencia.removeAll();
                                self.catAudiencia.push.apply(self.catAudiencia, catAudiencia);
                                $.ajax({
                                    url: "/Brief/GetAllClasificacionProyecto", // URL del método GetAll en tu API
                                    type: "GET",
                                    contentType: "application/json",
                                    success: function (d) {
                                        self.catClasificacionProyecto.removeAll();
                                        self.catClasificacionProyecto.push.apply(self.catClasificacionProyecto, d.datos.$values);
                                        $("#divEdicion").modal("hide");
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
    self.Editar = function (brief) {
        self.Limpiar();

        self.id(brief.id);
        $.ajax({
            url: "/Brief/Details/" + self.id(), // URL del método GetAll en tu API
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

                var EstatusBrief = self.catEstatusBrief().find(function (r) {
                    return r.id === d.datos.estatusBriefId;
                });
                self.EstatusBrief(EstatusBrief);
                var TipoBrief = self.catTipoBrief().find(function (r) {
                    return r.id === d.datos.tipoBriefId;
                });
                self.TipoBrief(TipoBrief);

                $("#divEdicion").modal("show");
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });

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
            url: "/Brief/EditStatus", // URL del método GetAll en tu API
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
        self.rutaArchivo("");
        self.cargaArchivo("");
    }
    self.GuardarProyecto = function () {
        var PlanComunicacion = false;
        if (self.determinarEstado() === "Sí") {
            PlanComunicacion = true;
        }
        var proyecto = {
            BriefId: self.id(),
            EstatusBriefId: self.EstatusBrief().id,
            ClasificacionProyectoId: self.ClasificacionProyecto().id,
            Comentario: self.comentarioProyecto(),
            Estado: self.determinarEstado(),
            RequierePlan: PlanComunicacion,
            FechaPublicacion: self.fechaPublicacion()
        };
       
       
        $.ajax({
            url: "/Brief/CreateProyecto",
            type: "POST",
            contentType: "application/json",  // Cambiado a JSON
            data: JSON.stringify(proyecto),  // Convertir a JSON
            success: function (d) {
                self.inicializar();
                $("#divEdicion").modal("hide");
                $('#alertMessage').text(d.mensaje);
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
    }
    self.GuardarMaterial = function () {

        var formData = new FormData();
        formData.append("BriefId", self.id().id);
        formData.append("Nombre", self.nombreMaterial());
        formData.append("Mensaje", self.mensaje());
        formData.append("PrioridadId", self.prioridad().id);
        formData.append("Ciclo", self.ciclo());
        formData.append("PCNId", self.pcn().id);
        formData.append("AudienciaId", self.audiencia().id);
        formData.append("FormatoId", self.formato().id);
        formData.append("FechaEntrega", self.fechaEntrega());
        formData.append("Proceso", self.proceso());
        formData.append("Produccion", self.produccion());
        formData.append("Responsable", self.responsable());

        $.ajax({
            url: "/Brief/CreateMaterial", // URL del método GetAll en tu API
            type: "POST",
            contentType: false,  // Important to avoid jQuery processing data
            processData: false,  // Important to avoid jQuery processing data
            data: formData,
            success: function (d) {
                self.inicializar();
                $("#divEdicion").modal("hide");
                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Success");
                $("#alertModal").modal("show");
                self.Limpiar();
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
    }
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

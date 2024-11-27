function AppViewModel() {
    var self = this;

    self.columns = ko.observableArray();
    self.columns2 = ko.observableArray();

    self.id = ko.observable(0);
    self.nombre = ValidationModule.validations.requiredField();
    self.descripcion = ValidationModule.validations.requiredField();
    self.objetivo = ValidationModule.validations.requiredField();
    self.dirigidoA = ValidationModule.validations.requiredField();
    self.comentario = ValidationModule.validations.requiredField();
    self.rutaArchivo = ValidationModule.validations.requiredField();
    self.fechaEntrega = ValidationModule.validations.requiredField();

    self.catEstatusBrief = ko.observableArray();
    self.EstatusBrief = ko.observable();
    self.catTipoBrief = ko.observableArray();
    self.TipoBrief = ValidationModule.validations.requiredField();
    self.cargaArchivo = ko.observable();
    self.registros = ko.observableArray();
    self.linksReferencias = ValidationModule.validations.requiredField();

    self.filtroNombre = ko.observable(""); // Texto del filtro

    self.errors = ko.validation.group(self);

    // Computado para devolver los registros filtrados
    self.registrosFiltrados = ko.computed(function () {
        var filtro = self.filtroNombre().toLowerCase();
        if (!filtro) {
            return self.registros(); // Sin filtro, devuelve todos los registros
        }
        return ko.utils.arrayFilter(self.registros(), function (item) {
            return item.nombre.toLowerCase().includes(filtro);
        });
    });

    self.inicializar = function () {
        $.ajax({
            url: "Brief/GetAllbyUserBrief", // URL del método GetAll en tu API
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                self.registros.removeAll();
                self.registros.push.apply(self.registros, d.datos);
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
    };

    self.inicializar();

    // Método para comprobar si el rol actual coincide con el pasado
    self.isRoleVisible = function (allowedRoles) {
        return allowedRoles.includes(RolId);
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
    self.Agregar = function () {
        self.Limpiar();
        var EstatusBrief = self.catEstatusBrief().find(function (r) {
            return r.id === 1;
        });
        self.EstatusBrief(EstatusBrief);
        $("#divEdicion").modal("show");

    }
    self.Guardar = function () {
        
        if (self.id() === 0) {
            self.GuardarNuevo();
        }
        else {
            self.GuardarEditar();
        }

    }
    self.Limpiar = function () {
        self.id(0);
        self.nombre("");
        self.descripcion("");
        self.objetivo("");
        self.dirigidoA("");
        self.rutaArchivo("");
        self.fechaEntrega("");
        self.rutaArchivo("");
        self.cargaArchivo("");
        self.linksReferencias("");
    }
    self.Editar = function (brief) {
        self.Limpiar();
       
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

                $("#divEdicion").modal("show");
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
       
    }
    self.GuardarEditar = function () {
        $("#divEdicion").modal("hide");
        var formData = new FormData();

        formData.append("Id", self.id());
        formData.append("Nombre", self.nombre());
        formData.append("Descripcion", self.descripcion());
        formData.append("Objetivo", self.objetivo());
        formData.append("DirigidoA", self.dirigidoA());
        formData.append("FechaEntrega", self.fechaEntrega());
        formData.append("EstatusBriefId", self.EstatusBrief().id);
        formData.append("TipoBriefId", self.TipoBrief().id);
        formData.append("LinksReferencias", self.linksReferencias());

        
        // Solo agregar el archivo si se ha seleccionado uno
        if (self.cargaArchivo()) {
            formData.append("Archivo", self.cargaArchivo());
        }

        $.ajax({
            url: "Brief/EditBrief", // URL del método GetAll en tu API
            type: "POST",
            contentType: false,  // Important to avoid jQuery processing data
            processData: false,  // Important to avoid jQuery processing data
            data: formData,
            success: function (d) {
                self.inicializar();
              
                $('#alertMessage').text(d.mensaje);
                $('#alertModalLabel').text("Success");
                $("#alertModal").modal("show");
                self.Limpiar();
                $(document).ajaxStop(function () {
                    $('#loader').addClass('d-none');
                });
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                $('#alertMessage').text("Error al obtener los datos: " + xhr.responseText);
                $('#alertModalLabel').text("Error");
                $("#alertModal").modal("show");
            }
        });
    }
    self.GuardarNuevo = function () {

        $("#divEdicion").modal("hide");
        var formData = new FormData();
       
        formData.append("Nombre", self.nombre());
        formData.append("Descripcion", self.descripcion());
        formData.append("Objetivo", self.objetivo());
        formData.append("DirigidoA", self.dirigidoA());
        formData.append("FechaEntrega", self.fechaEntrega());
        formData.append("EstatusBriefId", self.EstatusBrief().id);
        formData.append("TipoBriefId", self.TipoBrief().id);
        formData.append("LinksReferencias", self.linksReferencias());

        // Solo agregar el archivo si se ha seleccionado uno
        if (self.cargaArchivo()) {
            formData.append("Archivo", self.cargaArchivo());
        }
       
        $.ajax({
            url: "Brief/AddBrief", // URL del método GetAll en tu API
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
    // Función para leer el filtro desde el query string y asignarlo
   
}

// Inicializa SortableJS después de que Knockout haya sido inicializado
 function setFiltroFromQueryString(viewModel) {
        const params = new URLSearchParams(window.location.search);
        const filtro = params.get("filtroNombre"); // Nombre del parámetro en el query string
        if (filtro) {
            viewModel.filtroNombre(filtro); // Asigna el valor al filtro
        }
    }
var appViewModel = new AppViewModel();
ko.applyBindings(appViewModel);
setFiltroFromQueryString(appViewModel);
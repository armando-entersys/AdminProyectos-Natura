function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray([]);
    self.id = ko.observable(0);
    self.fechaEntrega = ko.observable("");
    self.nombre = ko.observable("");
    self.responsable = ko.observable("");
    self.area = ko.observable("");

    // Initialize the calendar
    self.inicializarCalendario = function () {
        var calendarEl = document.getElementById('calendar');
        var calendar = new FullCalendar.Calendar(calendarEl, {
            locale: 'es', // Idioma español
            initialView: 'dayGridMonth', // Vista inicial
            headerToolbar: {
                left: 'prev,next today', 
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            events: self.registros(), // Cargar eventos
            eventDisplay: 'block',
            displayEventTime: false,
            eventClick: function (info) {
                self.fechaEntrega(info.event.extendedProps.fechaEntrega);
                self.nombre(info.event.title);
                self.responsable(info.event.extendedProps.responsable);
                self.area(info.event.extendedProps.area);
                $("#divEdicion").modal("show");
            },
            eventContent: function (arg) {
                let customHtml = `
                <div class="fc-event-title-container">
                    <div class="fc-event-title">${arg.event.title}</div>
                    <div class="fc-event-subtitle">
                        Fecha: ${arg.event.extendedProps.fechaEntrega}<br>
                        Responsable: ${arg.event.extendedProps.responsable}<br>
                        Área: ${arg.event.extendedProps.area}
                    </div>
                </div>
            `;
                return { html: customHtml };
            }
        });
        calendar.render();
    };


    // Fetch project data and load into the calendar
    self.inicializar = function () {
        $.ajax({
            url: "Materiales/ObtenerMateriales",
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                var projects = d.datos.map(function (item) {
                    var fechaFormateada = new Date(item.fechaEntrega).toISOString().split('T')[0];
                    var fechaFin = new Date(item.fechaEntrega);
                    fechaFin.setDate(fechaFin.getDate() + 1); // +1 día como ejemplo
                    return {
                        title: item.nombre,
                        start: fechaFormateada,
                        end: fechaFin, 
                        allDay: true,
                        extendedProps: {
                            fechaEntrega: fechaFormateada,
                            responsable: item.responsable,
                            area: item.area
                        }
                    };
                });

                self.registros(projects); // Actualiza los datos
                self.inicializarCalendario(); // Renderiza el calendario
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
    };

    self.inicializar();
}

// Activate Knockout.js bindings
ko.applyBindings(new AppViewModel());

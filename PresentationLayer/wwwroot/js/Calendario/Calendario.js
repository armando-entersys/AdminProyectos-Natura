function AppViewModel() {
    var self = this;

    self.registros = ko.observableArray();

    // Initialize the calendar
    self.inicializarCalendario = function () {
        var calendarEl = document.getElementById('calendar');
        var calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth', // Default to month view
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek' // Allows month, week, and day views
            },
            events: self.registros(), // Bind to the observable array
            eventDisplay: 'block', // Display events as blocks
            displayEventTime: false, // Oculta la hora en el evento
            eventClick: function (info) {
                alert('Project: ' + info.event.title); // Handle event click
            }
        });
        calendar.render();
    };

    // Fetch project data and load into the calendar
    self.inicializar = function () {
        $.ajax({
            url: "/Brief/GetAllbyUserId",
            type: "GET",
            contentType: "application/json",
            success: function (d) {
                var projects = d.datos.$values.map(function (item) {
                    return {
                        title: item.nombre,
                        start: item.fechaEntrega, // Project's delivery date
                        description: item.descripcion
                    };
                });

                self.registros(projects); // Update observable with projects
                self.inicializarCalendario(); // Initialize calendar with projects
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

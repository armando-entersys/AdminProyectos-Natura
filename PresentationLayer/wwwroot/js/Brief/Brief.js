function Task(id, title, elemento) {
    this.id = id;
    this.title = ko.observable(title);
    this.elemento = elemento;
}

function Column(id, name, tasks) {
    this.id = id;
    this.name = ko.observable(name);
    this.tasks = ko.observableArray(tasks);
}

function AppViewModel() {
    var self = this;

    self.columns = ko.observableArray();
    self.columns2 = ko.observableArray();
    self.nombre = ko.observable();

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
                        return new Task(taskData.id, taskData.title, taskData.elemento);
                    });
                    return new Column(columnData.id, columnData.name, tasks);
                });

                self.columns.push.apply(self.columns, transformedColumns); // Añadimos las columnas transformadas

                // Inicializa SortableJS una vez que los datos han sido cargados
                initializeSortable();
            },
            error: function (xhr, status, error) {
                console.error("Error al obtener los datos: ", error);
                alert("Error al obtener los datos: " + xhr.responseText);
            }
        });
    };

    self.inicializar();

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

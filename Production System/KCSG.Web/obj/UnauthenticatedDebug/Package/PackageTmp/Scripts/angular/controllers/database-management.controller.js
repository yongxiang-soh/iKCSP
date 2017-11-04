angular.module("MasterDatabaseModule", ['ui.bootstrap', 'MasterDataTableService', 'ToastrNotification'])
        .controller("MasterDatabaseController", ['$scope', 'masterDataTable', 'toastrService',
            function ($scope, masterDataTable, toastrService) {

                // Define selection that user can make.
                $scope.tableSelection = {
                    tables: null, // Table which is selected from the dropdownlist.
                    records: [50, 100] // Total record that user can view per page.
                }

                // Instance which stores all selections made by user on page to filter table.
                $scope.selectedTableView = {
                    table: null,
                    page: 1,
                    records: $scope.tableSelection.records[0]
                };

                // Item which is needed to be inserted to database. 
                $scope.insertItem = {
                    data: null,
                    validationMessages: null
                };

                // Item which is needed deleting from database system.
                $scope.selectedRow = null;

                // Item which will be updated into database system.
                $scope.updateItem = null;

                // Instance represents the table which is currently displayed on the page.
                $scope.displayedTable = null;

                // This function is for loading table from service.
                $scope.loadTables = function () {

                    // Load tables list by using service function.
                    masterDataTable.loadTablesListAsync()
                    .then(
                    function (response) {
                        $scope.tableSelection.tables = response.data;
                        $scope.$applyAsync();
                    },
                    function (response) {

                        // This function is called when update is failed.
                        var data = response.data;
                        if (data == null)
                            return;

                        // Bad request is responded.
                        if (data.status == 400) {
                            // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                            toastrService.initializeToastrValidationMessages(data.validationMessages, null);
                        } else {
                            if (data.Message != null)
                                showMessage(data);
                        }

                    });

                }

                // Whether the selected item is being updated.
                $scope.inUpdateMode = false;

                // This function is for loading table columns and rows from service.
                $scope.loadTableRecords = function (table, page, records) {

                    // Clear the selected items.
                    $scope.selectedRow = null;
                    $scope.insertItem = {
                        data: null,
                        validationMessages: null
                    };

                    $scope.$applyAsync();

                    masterDataTable.loadTableRecordsAsync(table, page, records)
                        .then(
                        function (response) {
                            var data = response.data;
                            $scope.refineTableRecords(data.Headers, data.Body.Rows);
                            $scope.displayedTable = data;

                            // Copy header to insert item, the header provides necessary information for data being inputed.
                            $scope.insertItem.data = angular.copy(data.Headers);

                            // Due to the lack of value field, it should be appended in insert item.
                            for (var i = 0; i < $scope.insertItem.length; i++) {
                                $scope.insertItem.data[i]['Value'] = null;
                            }

                            $scope.$applyAsync();
                        },
                        function (response) {

                            // This function is called when update is failed.
                            var data = response.data;
                            if (data == null)
                                return;

                            // Bad request is responded.
                            if (data.status == 400) {

                                // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                                toastrService.initializeToastrValidationMessages(data.validationMessages, null);
                            } else {
                                if (data.Message != null)
                                    showMessage(data);
                            }
                        });
                }

                // This function is for inserting a record into database.
                $scope.insertTableRecord = function () {
                    masterDataTable.insertRecordAsync($scope.selectedTableView.table.Name, $scope.insertItem.data)
                        .then(
                        function () {
                            // Close data insert dialog.
                            $('#insertRecordModal').modal('hide');
                        },
                        function (response) {

                            // Close data insert dialog.
                            $('#insertRecordModal').modal('hide');

                            // This function is called when update is failed.
                            var data = response.data;
                            if (data == null)
                                return;

                            // Bad request is responded.
                            if (data.status == 400) {

                                // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                                toastrService.initializeToastrValidationMessages(data.validationMessages, null);
                            } else {

                                if (data.Message != null)
                                    showMessage(data);
                            }
                        });
                }

                // This function is called when an item is selected on table.
                $scope.selectItem = function (item) {

                    // Item has been selected before. Unselect it.
                    if ($scope.selectedRow != null) {
                        // Item is selected again.
                        if ($scope.selectedRow == item) {

                            // In item update mode, user is allowed to click on the same row to modify data.
                            if (!$scope.inUpdateMode) {
                                $scope.selectedRow = null;
                                $scope.updateItem = null;
                            }
                        }
                        else {
                            $scope.selectedRow = item;
                            $scope.updateItem = angular.copy($scope.selectedRow);
                        }
                    } else {
                        $scope.selectedRow = item;
                        $scope.updateItem = angular.copy($scope.selectedRow);
                    }

                    $scope.$applyAsync();
                }

                // This event is fired when an item is selected to be deleted.
                $scope.confirmDeleteItem = function () {

                    // No item has been selected.
                    if ($scope.selectedRow == null)
                        return;

                    // Re-format the paramters before send them to server.
                    var parameters = angular.copy($scope.selectedRow);

                    // Find the collection of object keys.
                    var keys = Object.keys(parameters);

                    // Go through every parameter, as it is an instance of Date, convert it to ISO Date string.
                    for (var i = 0; i < keys.length; i++) {
                        var key = keys[i];
                        if (angular.isDate(parameters[key])) {
                            parameters[key] = moment(parameters[key]).format('YYYY-MM-DD HH:mm:ss.SSS');
                        }
                    }
                    masterDataTable
                        .deleteItemAsync($scope.selectedTableView.table.Name, parameters)
                        .then(
                        function () {
                            // As the deletion is successful. Reload the page.
                            $scope.pageChanged();
                        },
                        function () {
                            // This function is called when update is failed.
                            var data = response.data;
                            if (data == null)
                                return;

                            // Bad request is responded.
                            if (data.status == 400) {

                                // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                                toastrService.initializeToastrValidationMessages(data.validationMessages, null);
                            } else {
                                if (data.Message != null)
                                    showMessage(data);
                            }
                        });
                }

                // This function is for toggling item update mode.
                $scope.toggleItemUpdate = function () {

                    // Delete, create button should be enabled when update mode is off and vice versa.
                    $scope.inUpdateMode = !$scope.inUpdateMode;
                    $scope.$applyAsync();
                }

                // This function is for cancelling item from being update.
                // Item will be reset to its original data, row is off update.
                $scope.cancelItemUpdate = function () {
                    // Reset selected row to null to make every item be unselected.
                    $scope.selectedRow = null;

                    // No item should be updated.
                    $scope.updateItem = null;

                    // Apply changes.
                    $scope.$applyAsync();
                }

                // Item is confirmed to be updated.
                // Gather item information and request service to update.
                $scope.confirmItemUpdate = function () {

                    // No item has been selected.
                    if ($scope.selectedRow == null)
                        return;

                    // Re-format the paramters before send them to server.
                    var parameters = angular.copy($scope.selectedRow);

                    // Find all parameters key.
                    var keys = Object.keys(parameters);

                    // Go through every parameter, as it is an instance of Date, convert it to ISO Date string.
                    for (var i = 0; i < keys.length; i++) {
                        var key = keys[i];
                        if (angular.isDate(parameters[key])) {
                            parameters[key] = moment(parameters[key]).format('YYYY-MM-DD HH:mm:ss.SSS');
                        }
                    }

                    // Call update function from service to do SQL record update.
                    masterDataTable.updateItemAsync($scope.selectedTableView.table.Name,
                        parameters,
                        angular.copy($scope.updateItem))
                    .then(
                    function () {
                        // As the update is successful. Page should be reloaded.
                        $scope.pageChanged();
                    },
                    function (response) {
                        // This function is called when update is failed.
                        var data = response.data;
                        if (data == null)
                            return;

                        // Bad request is responded.
                        if (data.status == 400) {
                            // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                            toastrService.initializeToastrValidationMessages(data.validationMessages, null);
                        } else {
                            console.log(data);
                            if (data.Message != null)
                                showMessage(data);
                        }
                    });
                }

                // This function is for refreshing table.
                $scope.clickRefresh = function () {

                    if ($scope.selectedTableView == null ||
                        $scope.selectedTableView.table == null ||
                        $scope.selectedTableView.length < 1)
                        return;

                    // Request loading data from service.
                    $scope.loadTableRecords($scope.selectedTableView.table.Name,
                        0,
                        $scope.selectedTableView.records);
                }

                // This function is called when user presses on a pagination to move forward or backward.
                $scope.pageChanged = function () {

                    // Request loading data from service.
                    $scope.loadTableRecords($scope.selectedTableView.table.Name,
                        $scope.selectedTableView.page - 1,
                        $scope.selectedTableView.records);
                }


                // This function is for finding suitable input type from SQL data type responded from server.
                $scope.findInputType = function (dataType) {
                    // Cast datatype to lower case.
                    var loweredCaseDataType = dataType.toLowerCase();

                    // Data is text.
                    if (loweredCaseDataType == "char" ||
                        loweredCaseDataType == "varchar" ||
                        loweredCaseDataType == "nvarchar" ||
                        loweredCaseDataType == "text" ||
                        loweredCaseDataType == "nchar" ||
                        loweredCaseDataType == "ntext")
                        return "text";

                    if (loweredCaseDataType == "datetime" || loweredCaseDataType == "smalldatetime")
                        return "datetime-local";

                    if (loweredCaseDataType == "date")
                        return "datetime-local";

                    if (loweredCaseDataType == "time")
                        return "datetime-local";

                    return "number";
                }

                // Refine rows data base on their related columns.
                $scope.refineTableRecords = function (columns, rows) {

                    for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
                        // Find the current iterated row.
                        var row = rows[rowIndex];

                        // Refine the row.
                        $scope.refineTableRow(columns, row);
                    }
                }

                // Base on the column type, refine a collection of rows.
                $scope.refineTableRow = function (columns, row) {

                    for (var columnIndex = 0; columnIndex < columns.length; columnIndex++) {

                        // Find the current iterated column.
                        var column = columns[columnIndex];

                        // Refine the cell.
                        $scope.refineTableCell(column, row);
                    }
                }

                // Base on the column type, data should be refined. This function is applied to special cases such as : DateTime, ...
                $scope.refineTableCell = function (column, row) {

                    // Column is invalid.
                    if (column == null)
                        return;

                    // Column name is not valid.
                    if (column.Column == null)
                        return;

                    // Row is invalid.
                    if (row == null)
                        return;

                    // Cell is already an object.
                    if (angular.isObject(row[column.Column]))
                        return;

                    // Column type is datetime
                    switch ($scope.findInputType(column.Type)) {
                        case "datetime-local":

                            // Build a regular expression for checking datetime returned from server whether it matches with /Date(<numeric>)/ or not.
                            // As it does, convert it back to Date instance.
                            var sqlDateRegex = /[/]Date[(]([0-9]|[-][0-9])+[)][/]/gi;
                            if (sqlDateRegex.test(row[column.Column])) {

                                // Initialize a regular expression to keep the numbers only.
                                var regexKeepNumber = /[^0-9.]/gi;

                                // Refine the date to number string.
                                row[column.Column] = row[column.Column].replace(regexKeepNumber, "");

                                try {

                                    // From the number sequence, parse and initialize Date instance.
                                    row[column.Column] = new Date(parseInt(row[column.Column]));
                                } catch (exception) {
                                    row[column.Column] = null;
                                }
                            }
                        default:
                            break;
                    }
                }

                // Base on the format of column, data should be displayed correctly.
                $scope.findDataDisplay = function (data) {

                    // Data is null.
                    if (data == null)
                        return null;

                    // Data is date.
                    if (angular.isDate(data))
                        return data.toLocaleString();

                    return data.toString();
                }

                // This function is for generating GUID.
                $scope.initializeGuid = function () {
                    function s4() {
                        return Math.floor((1 + Math.random()) * 0x10000)
                          .toString(16)
                          .substring(1);
                    }
                    return s4() + s4() + s4() + s4() +
                      s4() + s4() + s4() + s4();

                }
            }]);
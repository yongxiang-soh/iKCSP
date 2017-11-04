angular.module("MasterDatabaseModule", ['ui.bootstrap', 'ManagementReportConsumerService', 'ToastrNotification'])
        .controller("ManagementReportConsumerController", ['$scope', 'managementReportConsumerTable', 'toastrService',
            function ($scope, managementReportConsumer, toastrService) {
                // Define selection that user can make.
                $scope.tableSelection = {
                    tables: null, // Table which is selected from the dropdownlist.
                    records: [5, 10, 15] // Total record that user can view per page.
                }

                // Instance which stores all selections made by user on page to filter table.
                $scope.selectedTableView = {
                    table: null,
                    page: 1,
                    records: $scope.tableSelection.records[0]
                };

                // Item which is needed deleting from database system.
                $scope.selectedRow = null;

                // Item which will be updated into database system.
                $scope.updateItem = null;

                // Instance represents the table which is currently displayed on the page.
                $scope.displayedTable = null;
                $scope.Total = null;
                // This function is for loading table from service.
                $scope.loadTables = function () {

                    // Load tables list by using service function.
                    managementReportConsumer.loadTablesListAsync()
                    .then(
                    function (response) {
                        $scope.tableSelection.tables = response.data;
                        $scope.Total = response.data.itemsCount;
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
                $scope.loadTableRecords = function (yearmonth,page,matcode) {
                    $.ajax({
                        url: '/ManagementReport/MaterialReceivedConsumedList/Checkitemexist',
                        type: 'post',
                        data: {yearmonth:yearmonth},
                        success: function (response) {
                            if (!response) {
                                var ok =
                                    confirm("Cannot find relative records from database! Ready to calculate the received/used total of this month?");
                                if (ok) {
                                    $scope.$applyAsync();

                                    managementReportConsumer.CalculateTableRecordsAsync(yearmonth,matcode)
                                        .then(
                                            function(response) {
                                                var data = response.data.data;
                                                $scope.displayedTable = data;
                                                $scope.Total = response.data.itemsCount;
                                                $scope.$applyAsync();
                                                $scope.selectedRow = null;
                                            },
                                            function(response) {

                                                // This function is called when update is failed.
                                                var data = response.data;
                                                if (data == null)
                                                    return;

                                                // Bad request is responded.
                                                if (data.status == 400) {

                                                    // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                                                    toastrService
                                                        .initializeToastrValidationMessages(data.validationMessages,
                                                            null);
                                                } else {
                                                    if (data.Message != null)
                                                        showMessage(data);
                                                }
                                            });

                                } else {
                                    return;
                                }
                            } else {
                                
                                $scope.$applyAsync();

                                managementReportConsumer.loadTableRecordsAsync(yearmonth,page)
                                    .then(
                                        function (response) {
                                            var data = response.data.data;
                                            $scope.displayedTable = data;
                                            $scope.Total = response.data.itemsCount;
                                            $scope.$applyAsync();
                                            $scope.selectedRow = null;
                                        },
                                        function (response) {

                                            // This function is called when update is failed.
                                            var data = response.data;
                                            if (data == null)
                                                return;

                                            // Bad request is responded.
                                            if (data.status == 400) {

                                                // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                                                toastrService
                                                    .initializeToastrValidationMessages(data.validationMessages, null);
                                            } else {
                                                if (data.Message != null)
                                                    showMessage(data);
                                            }
                                        });


                            }
                        }
                            
                    });

                }

                // This function is for recalculate item.
                $scope.Recalculate = function (yearmonth,matcode) {


                    $scope.$applyAsync();

                    managementReportConsumer.CalculateTableRecordsAsync(yearmonth,matcode)
                        .then(
                            function (response) {
                                var data = response.data.data;
                                $scope.displayedTable = data;
                                $scope.Total = response.data.itemsCount;
                                $scope.$applyAsync();
                                $scope.selectedRow = null;
                            },
                            function (response) {

                                // This function is called when update is failed.
                                var data = response.data;
                                if (data == null)
                                    return;

                                // Bad request is responded.
                                if (data.status == 400) {

                                    // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                                    toastrService
                                        .initializeToastrValidationMessages(data.validationMessages,
                                            null);
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
                    if ($scope.selectedRow !== null && $scope.selectedRow !== item) {
                        $scope.toggleItemUpdate();
                    } else {
                        if (!$scope.inUpdateMode) {

                            $scope.toggleItemUpdate();
                        }
                    }
                    $(".ng-pristine").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '999999.99', mDec: 2 });
                }

                // This function is for toggling item update mode.
                $scope.toggleItemUpdate = function () {
                    //var ok = confirm("Ready to update?");
                    //if (!ok) {
                    //    return;
                    //}

                    // Delete, create button should be enabled when update mode is off and vice versa.
                    $scope.inUpdateMode = true;
                    
                    if ($scope.inUpdateMode) {
                        $scope.updateItem = angular.copy($scope.selectedRow);
                        
                    }
                    $scope.$applyAsync();
                   
                }

                // This function is for cancelling item from being update.
                // Item will be reset to its original data, row is off update.
                $scope.cancelItemUpdate = function () {
                    // Reset selected row to null to make every item be unselected.
                    // Delete, create button should be enabled when update mode is off and vice versa.
                    $scope.inUpdateMode = false;

                    $scope.$applyAsync();
                }

                // Item is confirmed to be updated.
                // Gather item information and request service to update.
                $scope.confirmItemUpdate = function () {
                    var ok = confirm("Ready to update?");
                    if (!ok) {
                        $scope.cancelItemUpdate();
                        $scope.selectedRow = null;
                        return;
                    }
                    // No item has been selected.
                    if ($scope.selectedRow == null || $scope.updateItem == null)
                        return;

                    // Re-format the paramters before send them to server.
                    var parameters = angular.copy($scope.updateItem);
                    parameters['Updatedate'] = $scope.sqlDateToDateTime(parameters['Updatedate']);
                    parameters['YearMonth'] = $scope.sqlDateToDateTime(parameters['YearMonth']);

                    parameters['Updatedate'] = moment(parameters['Updatedate']).format('YYYY-MM-DD HH:mm:ss.SSS');
                    parameters['YearMonth'] = moment(parameters['YearMonth']).format('YYYY-MM-DD HH:mm:ss.SSS');

                    // TODO: Call ajax.
                    $.ajax({
                        url: '/ManagementReport/MaterialReceivedConsumedList/UpdateManagementReport',
                        type: 'post',
                        data: {
                            parameters: parameters
                        },
                        success: function (response) {
                            if (response) {
                                $scope.$applyAsync();

                                managementReportConsumer.loadTableRecordsAsync($("#YearMonth").val(), $scope.selectedTableView.page)
                                    .then(
                                        function(response) {
                                            var data = response.data.data;
                                            $scope.displayedTable = data;
                                            $scope.Total = response.data.itemsCount;
                                            $scope.$applyAsync();
                                            $("#Remain").autoNumeric('init', "vMax: '999999',mDec : 2");
                                        },
                                        function(response) {

                                            // This function is called when update is failed.
                                            var data = response.data;
                                            if (data == null)
                                                return;

                                            // Bad request is responded.
                                            if (data.status == 400) {

                                                // The response data should be validation messages. Therefore, they should be displayed on pop-up.
                                                toastrService
                                                    .initializeToastrValidationMessages(data.validationMessages, null);
                                            } else {
                                                if (data.Message != null)
                                                    showMessage(data);
                                            }
                                        });
                            } else {
                                alert("Something went wrong!.");
                            }

                        }
                    });
                    $scope.cancelItemUpdate();
                }

                // This function is for refreshing table.
                $scope.clickRefresh = function () {

                    if ($("#YearMonth").val() == "")
                        return;
                    var page = 1;
                    // Request loading data from service.
                    $scope.loadTableRecords($("#YearMonth").val(), page);
                    
                    $("#Remain").autoNumeric('init', "vMax: '999999',mDec : 2");
                }
                // This function is for print table.
                $scope.PrintItem = function (yearmonth) {
                    if (!confirm("Ready to print?")) {
                        return;
                    }
                    if ($("#YearMonth").val() == "")
                        return;
                    $.ajax({
                        url: '/ManagementReport/MaterialReceivedConsumedList/ExportConsumerMaterial',
                        type: 'post',
                        data: { yearmonth: $("#YearMonth").val() },
                        success: function(response) {
                            var render = response.render;

                            if (render != null) {

                                $("#PrintArea")
                                    .html(render)
                                    .show()
                                    .print()
                                    .empty()
                                    .hide();
                            }
                        }
                    });

                }
                // This function is for recalculate table.
                $scope.reloaditem = function () {
                    if ($("#YearMonth").val() == "")
                        return;
                    if ($scope.selectedRow === null) {
                        return;
                    };
                    var ok = confirm("Ready to Re-calculate the received/used amount of this selected record?");
                    if (!ok) {
                        $scope.cancelItemUpdate();
                        $scope.selectedRow = null;
                        return;
                    }



                    // Request loading data from service.
                    $scope.Recalculate($scope.selectedRow.YearMonthst, $scope.selectedRow.MaterialCode);
                    $scope.cancelItemUpdate();
                    $scope.selectedRow = null;
                }
                // This function is called when user presses on a pagination to move forward or backward.
                $scope.pageChanged = function () {

                    // Request loading data from service.
                    $scope.loadTableRecords($("#YearMonth").val(), $scope.selectedTableView.page);
                },

                // Format SQL Datetime responded from service to screen.
                $scope.dislayDateTime = function(sqlDateTime) {
                    return gridHelper.displayDateTime(sqlDateTime);
                }

                // Convert datetime formatted by SQL to DateTime instance of Javascript.
                $scope.sqlDateToDateTime = function (dateTime) {
                    var sqlDateRegex = /[/]Date[(]([0-9]|[-][0-9])+[)][/]/gi;
                    if (sqlDateRegex.test(dateTime)) {

                        // Initialize a regular expression to keep the numbers only.
                        var regexKeepNumber = /[^0-9.]/gi;

                        // Refine the date to number string.
                        var unix = dateTime.replace(regexKeepNumber, "");

                        try {

                            // From the number sequence, parse and initialize Date instance.
                            return new Date(parseInt(unix));
                        } catch (exception) {
                            return null;
                        }
                    }
                }
            }]);
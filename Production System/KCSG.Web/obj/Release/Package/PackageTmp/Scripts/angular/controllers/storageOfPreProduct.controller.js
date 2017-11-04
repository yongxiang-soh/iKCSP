angular.module("RetrieveEmptyContainerModule", [])
    .controller("RetrieveEmptyContainerController",
    [
        '$scope', '$http', function ($scope, $http) {
            $('#Quantity').keyup(function () {
                var hasValue = this.value.length > 0;
                $("#errorText").text("").hide();
                $("#Quantity").removeClass('input-validation-error');
            });

            // By default, reset the container to default settings.
            $scope.container = null;

            // This flag is for indicating whether item is waiting for being retrieved or not.
            $scope.storageMode = 0;

            // This flag indicates whether data is being storaged or not.
            $scope.isStoraging = false;

            // This function is for reseting container settings.
            $scope.resetContainer = function () {

                // Data is not being storaged.
                $scope.isStoraging = false;

                $scope.container = {
                    KneadingLine: 1,
                    Colour: null,
                    Temperature: null,
                    CommandNo: null,
                    PreProductCode: null,
                    PreProductName: null,
                    ContainerNo: null,
                    ContainerType: null,
                    ContainerCode: null,
                    PreProductLotNo: null,
                    StoragedContainerQuantity: null,
                    Conveyor: null,
                    ShelfRow: null,
                    ShelfBay: null,
                    ShelfLevel: null,
                    Status: null,
                    PutQuantity: 0
                };

                $scope.$applyAsync();
            },
                $scope.onInit = function () {
                    // Reset the container first.
                    $scope.resetContainer();

                    // Find the container from server.
                    $scope.loadEmptyContainer();
                },

            // This function is called when the kneading line is changed.
                $scope.kneadingLineChange = function () {
                    // Find the form information by using kneading line information.
                    $scope.loadEmptyContainer();
                },

            // This function is for finding empty container by using KneadingLine information.
            // Please refer BR9 for more information.
                $scope.loadEmptyContainer = function () {

                    // Close retrieval.
                    $scope.storageMode = 0;
                    $scope.$applyAsync();

                    var promise = $scope.findEmptyContainer($scope.container.KneadingLine);
                    promise.then(

                        // Success response handler.
                        function (response) {

                            // Find the response data.
                            var data = response.data.container;

                            //	If no record found, the system shows form as blank, shows the button [Retrieval] as non-editable
                            if (data == null) {
                                // Backup the kneading line option.
                                var kneadingLine = $scope.container.KneadingLine;
                                $scope.resetContainer();
                                $scope.container.KneadingLine = kneadingLine;
                                $scope.$applyAsync();
                                showMessage(response.data);
                                $("#btnRetrieval").prop("disabled", true);
                                return;
                            }
                            $("#btnRetrieval").prop("disabled", false);

                            // Copy the object.
                            $scope.container = $scope.copyProperties(data, $scope.container);
                            $scope.$applyAsync();


                            //	The system will retrieve the conveyor with the selected [Line] as follow
                            var findConveyorPromise = $scope
                                .findConveyor($scope.container.KneadingLine, $scope.container.Colour);

                            // Promise handle.
                            findConveyorPromise.then(
                                function (response) {
                                    var conveyor = response["data"]["conveyor"];
                                    $scope.container.Conveyor = conveyor;

                                    if (conveyor == null) {
                                        var errorMessage20 = { Success: false, Message: "Conveyor status error." }
                                        showMessage(errorMessage20);
                                        $("#btnRetrieval").prop("disabled", true);
                                        return;
                                    }
                                },
                                function (response) { }
                            );
                        },

                        // Failure response handler
                        function (response) {
                        });
                },

            // This function is for searching empty container by using kneading line.
                $scope.findEmptyContainer = function (kneadingLine) {

                    var data = {
                        kneadingLine: kneadingLine
                    };

                    return $http.post("/StorageOfPreProduct/FindEmptyContainers", data, null);
                },

            // This function is for searching for conveyor by using specific conditions.
                $scope.findConveyor = function (kneadingLine, color) {

                    // Build the struct of data which will be submitted to server.
                    var data = {
                        kneadingLine: kneadingLine,
                        colour: color
                    };

                    return $http.post("/StorageOfPreProduct/FindConveyor", data, null);
                },

            // This function is for searching for product shelf status by using container number and type.
                $scope.findPreProductShelfStatus = function (containerType) {
                    var data = {
                        containerType: containerType
                    }

                    return $http.post("/StorageOfPreProduct/FindPreProductShelfStatus", data, null);
                }

            // Copy related properties from source to target
            $scope.copyProperties = function (source, target) {

                // Copy the target first.
                var cloneSource = angular.copy(source);

                // Clone the target also.
                var cloneTarget = angular.copy(target);

                var keys = Object.keys(cloneSource);
                for (var i = 0; i < keys.length; i++) {
                    var key = keys[i];
                    cloneTarget[key] = cloneSource[key];
                }

                return cloneTarget;
            },

            // From the kneading line and color code to color name.
                $scope.getColorDisplay = function (kneadingLine, color) {

                    if (kneadingLine == 1) {
                        if (color == 0)
                            return "Black";

                        return "Colour";
                    }
                    return null;
                },

            // From temperature code to temperature display.
                $scope.getTemperatureDisplay = function (temperature) {
                    if (temperature == 0)
                        return "Low";

                    return "Normal";
                }

            // This function is fired when Retrieval button is clicked.
            $scope.retrieve = function () {

                if (!confirm("Ready to retrieve?"))
                    return;

                //$scope.storageMode = 1;
                //$scope.$applyAsync();
                var data = {
                    containerType: $scope.container.ContainerType,
                    kneadingLine: $scope.container.KneadingLine,
                    colorClass: $scope.container.Colour
                }
                $http.post("/StorageOfPreProduct/Retrieval", data)
                    .then(function (response) {
                        if (!response.data.Success) {
                            showMessage(response.data);
                            $scope.storageMode = 0;
                            $scope.$applyAsync();
                            return;
                        } else {
                            console.log(response);
                            var row = response["data"]["result"]["ShelfRow"];
                            var bay = response["data"]["result"]["ShelfBay"];
                            var level = response["data"]["result"]["ShelfLevel"];

                            $scope.container.ContainerNo = response["data"]["result"]["ContainerNo"];
                            $scope.container.ContainerCode = response["data"]["result"]["ContainerCode"];
                            $scope.container.ContainerType = response["data"]["result"]["ContainerType"];
                            $scope.container.ShelfRow = row;
                            $scope.container.ShelfBay = bay;
                            $scope.container.ShelfLevel = level;
                            $("#btnRetrieval").prop("disabled", true);
                            $scope.storageMode = 1;
                            //$scope.$applyAsync();
                            blockScreenAccess('Retrieving from [' + row + '-' + bay + '-' + level + ']...');
                        }


                    });
            },

            // This function is fired when OK button is clicked on Store Pre-product into Warehouse using empty Container (TCIP022F)
                $scope.ok = function () {

                    if (!confirm("Ready to retrieve?"))
                        return;

                    var data = {
                        lsRow: $scope.container.ShelfRow,
                        lsBay: $scope.container.ShelfBay,
                        lsLevel: $scope.container.ShelfLevel,
                        containerNo: $scope.container.ContainerNo,
                        containerCode: $scope.container.ContainerCode,
                        kneadingLine: $scope.container.KneadingLine,
                        colorClass: $scope.container.Colour
                    };

                    $scope.isStoraging = true;
                    $scope.$applyAsync();

                    $http.post("/StorageOfPreProduct/PreProductStorageOk", data, { async: false })
                        .then(
                            function (response) {
                                $scope.isStoraging = false;
                                $scope.$applyAsync();
                                $("#IsOK").val(true);
                            },
                            function () {
                                $scope.isStoraging = false;
                                $scope.$applyAsync();

                            });

                    var szMessage = "From CV214 to ";
                    if ($scope.container.KneadingLine == 1)
                        szMessage += "CV211";
                    else
                        szMessage += "CV212";
                    blockScreenAccess('Moving ' + szMessage);
                }

            // This function is fired when Storage button is clicked.
            $scope.Storage = function () {
                var temperature = $scope.container.Temperature;
                var lotEndFlag = $('input[name="PutQuantity"]:checked').val();
                //alert(temperature);
                //var quantity = $scope.resetContainer.PutQuantity;
                var quantity = $("#storageQuantity").val();
                if (quantity === "" || quantity === null) {
                    $("#errorText").text("Please input data for this required field.").show();
                    $("#Quantity").addClass('input-validation-error');
                    return;
                }

                if (parseFloat(quantity) < 0.001) {
                    $("#errorText").text("Pack quantity must be more than zero.").show();
                    $("#Quantity").addClass('input-validation-error');
                    return;
                }
                $("#errorText").text("").hide();
                $("#Quantity").removeClass('input-validation-error');

                $scope.printLabel();

                $.ajax({
                    url: "/StorageOfPreProduct/CheckValidateInMarkRetrievedContainer",
                    type: "post",
                    data: {
                        temperature: temperature,
                        preProductCode: $scope.container.PreProductCode
                    },
                    success: function (response) {
                        if (!response.Success) {
                            showMessage(response);
                        } else {
                            if (!confirm("Ready to store?")) {
                                $.unblockUI();
                                return;
                            }

                            $.ajax({
                                url: "/StorageOfPreProduct/Storage",
                                type: "post",
                                async: false,
                                data: {
                                    commandNo: $scope.container.CommandNo,
                                    containerCode: $scope.container.ContainerCode,
                                    preProductLotNo: $scope.container.PreProductLotNo,
                                    preProductCode: $scope.container.PreProductCode,
                                    quantity: parseFloat(quantity),
                                    containerNo: $scope.container.ContainerNo,
                                    containerType: $scope.container.ContainerType,
                                    lotEndFlag: lotEndFlag,
                                    row: $scope.container.ShelfRow,
                                    bay: $scope.container.ShelfBay,
                                    level: $scope.container.ShelfLevel,
                                    colorClass: $scope.container.Colour
                                },
                                success: function (response) {
                                    // TODO:
                                    // Open label re-publish panel.
                                    //$scope.storageMode = 5;
                                    //$scope.$applyAsync();
                                    $scope.storageMode = 3;
                                    var szMessage = 'Storing into [' + $scope.container.ShelfRow + '-' + $scope.container.ShelfBay + '-' + $scope.container.ShelfLevel + ']';
                                    blockScreenAccess(szMessage);
                                   

                                }
                            });
                        }

                    }
                });
            }

            // This function is for checking whether storage be retrievable or not.
            $scope.isStorageRetrievable = function () {
                // Invalid kneading line.
                if ($scope.container.KneadingLine != 0 && $scope.container.KneadingLine != 1)
                    return false;

                // Invalid colour
                if ($scope.container.Colour == null)
                    return false;

                // Invalid temperature
                if ($scope.container.Temperature == null)
                    return false;

                // Invalid command number.
                if ($scope.container.CommandNo == null)
                    return false;

                // Invalid pre-product information.
                if ($scope.container.PreProductCode == null ||
                    $scope.container.PreProductLotNo == null ||
                    $scope.container.PreProductName == null)
                    return false;

                // Invalid container information.
                if ($scope.container.ContainerNo == null ||
                    $scope.container.ContainerType == null)
                    return false;

                // Invalid storaged quantity.
                if ($scope.container.StoragedContainerQuantity == null)
                    return false;

                return true;
            },

            // Whether ok button can be clicked or not.
                $scope.isStoragable = function () {

                    // Invalid shelf row.
                    if ($scope.container.ShelfRow == null || $scope.container.ShelfRow.length != 2)
                        return false;

                    // Invalid shelf bay.
                    if ($scope.container.ShelfBay == null || $scope.container.ShelfBay.length != 2)
                        return false;

                    // Invalid shelf level.
                    if ($scope.container.ShelfLevel == null || $scope.container.ShelfLevel.length != 2)
                        return false;
                    // Invalid container no.
                    if ($scope.container.ContainerNo == null)
                        return false;

                    // Invalid container type.
                    if ($scope.container.ContainerType == null)
                        return false;

                    if ($scope.isStoraging)
                        return false;

                    return true;
                },

            // This function is for re-storing containers.
                $scope.restore = function () {

                    // Display confirmation message.
                    if (!confirm("Ready to re-store?"))
                        return;

                    var data = {
                        lsRow: $scope.container.ShelfRow,
                        lsBay: $scope.container.ShelfBay,
                        lsLevel: $scope.container.ShelfLevel,
                        containerNo: $scope.container.ContainerNo,
                        containerCode: $scope.container.ContainerCode,
                        conveyor: $scope.container.Conveyor
                    };

                    $scope.isStoraging = true;

                    $http.post("/StorageOfPreProduct/PreProductStorageNg", data, { async: false })
                        .then(
                            function (response) {
                                $scope.isStoraging = false;
                                $scope.$applyAsync();
                                $("#IsOK").val(false);
                            },
                            function () {
                                $scope.isStoraging = false;
                                $scope.$applyAsync();

                            });
                    var szMessage = 'Storing into [' + $scope.container.ShelfRow + '-' + $scope.container.ShelfBay + '-' + $scope.container.ShelfLevel + ']';
                    blockScreenAccess(szMessage);
                };


            // Submit information to print label.
            $scope.printLabel = function () {

                var data = {
                    containerCode: $scope.container.ContainerCode,
                    commandNo: $scope.container.CommandNo,
                    preProductCode: $scope.container.PreProductCode,
                    lotNo: $scope.container.PreProductLotNo,
                    quantity: $scope.container.PutQuantity,
                    storageSeqNo: $scope.container.StoragedContainerQuantity,
                    preProductName: $scope.container.PreProductName
                }
                $http.post("/StorageOfPreProduct/Print", data)
                       .then(
                           function (response) {
//                               $.unblockUI();
                           },
                           function () {
                           });


            }

            // Initiate communication to c2 hub.
            $scope.signalrCommunication = initiateHubConnection(hubList.c2);

            // Message is sent from 
            $scope.signalrCommunication.client.receiveMessageFromC2 = function (message) {
                if (message == null)
                    return;

                if (message['PictureNo'] == null)
                    return;

                //if (message.PictureNo == "TCIP021F" || message.PictureNo == "TCIP022F") {
                if (message.PictureNo == "TCIP021F") {
                    
                    $.ajax({
                        url: "/StorageOfPreProduct/UC4",
                        type: "post",
                        data: {
                            preProductCode: $scope.container.PreProductCode
                        },
                        success: function (response) {

                            // Analyze C2 response and display messages.
                            findC2ResponseInformation(response);
                            $scope.container.Status = response.Status;
                            var isCheckStatus = false;
                            for (var i = 0; i < response.length; i++) {
                                if (response[i].F50_Status == '6') {
                                    $scope.storageMode = 1;
                                    isCheckStatus = true;
                                }
                            }
                            if (!isCheckStatus) {
                                $scope.container.ContainerNo = null;
                                $scope.container.ContainerCode = null;
                            }
                            $scope.$applyAsync();
                        },
                        done: function () {
                            // Unblock UI.
                            $.unblockUI();
                        }

                    });

                    return;
                }

                if (message.PictureNo == "TCIP022F") {
                    var isChecked = $("#IsOK").val();
                    
                    $.ajax({
                        url: "/StorageOfPreProduct/UC6",
                        type: "post",
                        data: {
                            isChecked: isChecked,
                            preProductCode: $scope.container.PreProductCode,
                            containerCode: $scope.container.ContainerCode
                        },
                        success: function (response) {

                            findC2ResponseInformation(response);
                            if (isChecked) {
                                for (var i = 0; i < response.length; i++) {
                                    if (response[i].F50_Status == '6') {
                                        $scope.putQuantityMode = 1;
                                    }
                                }
                            }
                            //if (!confirm("Ready to store?"))
                            //    return;
                            $scope.storageMode = 2;
                            $scope.$applyAsync();
                        }, 
                        done: function () {
                            // Unblock UI.
                            $.unblockUI();

                        }
                    });

                    return;
                }
                if (message.PictureNo == "TCIP023F") {
                    
                    $.ajax({
                        url: "/StorageOfPreProduct/UC7",
                        type: "post",
                        data: {
                            preProductCode: $scope.container.PreProductCode,
                            row: $scope.container.ShelfRow,
                            bay: $scope.container.ShelfBay,
                            level: $scope.container.ShelfLevel,
                            containerCode: $scope.container.ContainerCode,
                            containerNo: $scope.container.ContainerNo,
                            containerType: $scope.container.ContainerType,
                            commandNo: $scope.container.CommandNo,
                            preProductLotNo: $scope.container.PreProductLotNo

                        },
                        success: function (response) {

                            findC2ResponseInformation(response);

                            //if (!confirm("Ready to store?"))
                            //    return;
                            $scope.storageMode = 3;
                            $scope.$applyAsync();
                        },
                        done: function () {
                            $.unblockUI();
                        }
                    });
                }


            }

            // Start connection to hub.
            signalrStartHubConnection();
        }
    ]);
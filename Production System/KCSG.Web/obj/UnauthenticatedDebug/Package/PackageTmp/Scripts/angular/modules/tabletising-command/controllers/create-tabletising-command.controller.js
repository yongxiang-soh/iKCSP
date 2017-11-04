angular
    .module('CreateTabletisingCommandModule', ['TabletisingCommandService', 'ui.bootstrap'])
    .controller('CreateTabletisingCommandController', ['$scope', 'tabletisingCommandService',
        function ($scope, tabletisingCommandService) {

            // Kneading command search condition.
            $scope.kneadingCommandSearchCondition = {
                pageIndex: 1,
                pageSize: 30
            };

            // Product search condition.
            $scope.productSearchCondition = {
                pageIndex: 1,
                pageSize: 30
            }

            // List of kneading commands which should be displayed on table.
            $scope.kneadingCommandSearchResult = {
                commands: null,
                total: 0
            };

            // List of product and total number.
            $scope.productSearchResult = {
                products: null,
                total: 0
            };

            // List of product details.
            $scope.productDetailSearchResult = {
                productDetails: null,
                total: 0
            };

            $scope.productDetails = null; // List of product details.

            $scope.kneadingCommandSelect = null; // Command which is currently selected.
            $scope.productSelect = null; // Product which is currently selected.
            $scope.productDetailSelect = null; // Product detail which is currently selected.

            $scope.steps = CreateTabletisingStep;

            // By default, editor mode should be started from loading kneading command.
            $scope.editorMode = $scope.steps.LoadKneadingCommand;

            /**
             * Reset kneading commands search result.
             * @returns {} 
             */
            $scope.resetKneadingCommandSearchResult = function() {
                $scope.kneadingCommandSearchResult = {
                    commands: null,
                    total: 0
                };
            }

            /**
             * Reset product search result.
             * @returns {} 
             */
            $scope.resetProductSearchResult = function() {
                $scope.productSearchResult = {
                    products: null,
                    total: 0
                };
            }

            /**
             * Reset product details search result to original state.
             * @returns {} 
             */
            $scope.resetProductDetails = function () {
                $scope.productDetailSearchResult = {
                    productDetails: null,
                    total: 0
                };
            }

            /**
             * This function is called when component has been initiated successfully.
             * @returns {} 
             */
            $scope.init = function () {

                // Clear products list.
                $scope.resetProductDetails();
                $scope.resetProductSearchResult();
                $scope.resetKneadingCommandSearchResult();
                $scope.productDetailSelect = null;
                $scope.productSelect = null;
                $scope.kneadingCommandsTotal = 0;

                // Reset editor mode.
                $scope.editorMode = $scope.steps.LoadKneadingCommand;

                // Load list of kneading commands.
                $scope.loadKneadingCommands();
            }

            /**
             * This function is for loading kneading commands base on specific conditions.
             * @returns {} 
             */
            $scope.loadKneadingCommands = function (bPersistent) {
                // Clear all kneading command rows.
                if (!bPersistent)
                    $scope.resetKneadingCommandSearchResult();
                $scope.kneadingCommandSelect = null;

                // Search for kneading commands.
                tabletisingCommandService.loadKneadingCommandsList(angular.toJson($scope.kneadingCommandSearchCondition))
                    .then(

                    // This function is called when there is data returned from service.
                    function (x) {
                        // Response is invalid.
                        if (x == null || x['data'] == null)
                            return;
                        console.log(x);
                        // Only receive data.
                        x = x.data;
                        $scope.kneadingCommandSearchResult.total = x.itemsCount;
                        var rows = x.data;
                        if (!(rows instanceof Array))
                            return;

                        $scope.kneadingCommandSearchResult.commands = rows;
                        $scope.$applyAsync();
                    });
            }

            /**
             * This callback is fired when user clicks on delete button to delete a kneading command.
             * @param {} kneadingCommand 
             * @returns {} 
             */
            $scope.deleteKneadingCommand = function (kneadingCommand) {

                if (kneadingCommand.Status != '3') {
                    var x = { Message: 'Container has been taken out, tablet command cannot change.' };
                    showMessage(x);
                    return;
                }
                // Display confirmation dialog.
                if (!confirm("Ready to delete the corresponding tablet commands and tablet product commands?"))
                    return;

                // Send request to service to delete the specific command.
                tabletisingCommandService.deleteKneadingCommand(kneadingCommand)
                    .then(
                        // Success callback.
                        function (x) {
                            // Response is invalid.
                            if (x == null || x['data'] == null)
                                return;

                            // Only receive data.
                            x = x.data;

                            // Display message onto screen.
                            showMessage(x);

                            // Reload kneading commands list.
                            $scope.loadKneadingCommands();
                        });
            }

            /**
             * Select kneading command in list.
             * @param {} kneadingCommand 
             * @returns {} 
             */
            $scope.selectKneadingCommand = function (kneadingCommand) {

                // Kneading command is invalid.
                if (kneadingCommand == null)
                    return;

                // Initiate searching conditions.
                var conditions = {};
                Object.assign($scope.productSearchCondition, conditions);

                conditions['preproductCode'] = kneadingCommand.PreproductCode;
                tabletisingCommandService.loadProductInformation(conditions)
                .then(

                // Success callback.
                function (x) {

                    // Response is invalid.
                    if (x == null || x['data'] == null)
                        return;

                    $scope.productSearchResult.total = x.itemsCount;

                    // Bind data.
                    x = x.data;
                    
                    // Find products list.
                    $scope.productSearchResult.products = x.data;
                    console.log(x);
                    // Change editor mode.
                    $scope.editorMode = $scope.steps.LoadProductInformation;

                }, function () {

                });
            }

            /**
             * Can product details be loaded or not.
             * @returns {} 
             */
            $scope.canLoadProductDetails = function () {

                // Kneading command is invalid.
                if ($scope.kneadingCommandSelect == null)
                    return false;

                // Kneading command lot number is invalid.
                if ($scope.isNullOrEmpty($scope.kneadingCommandSelect.LotNo))
                    return false;

                // Command no is invalid.
                if ($scope.isNullOrEmpty($scope.kneadingCommandSelect.KneadingNo))
                    return false;

                // Pre-product code is invalid.
                if ($scope.isNullOrEmpty($scope.kneadingCommandSelect.PreproductCode))
                    return false;
                
                return true;
            }

            /**
             * This function is for loading product details of a specific product.
             * @returns {} 
             */
            $scope.loadProductDetails = function () {

                // Product details cannot be loaded.
                if (!$scope.canLoadProductDetails())
                    return;

                $scope.productDetailSearchResult = {};
                $scope.productDetailSearchResult['productDetails'] = [];
                
                var productDetails = {
                    ProductCode: $scope.productSelect.F09_ProductCode,
                    ProductName: $scope.productSelect.F09_ProductDesp,
                    TabletisingQuantity: 0,
                    UsedPreProduct: 0,
                    LotNo: $scope.kneadingCommandSelect.LotNo
                };

                var remain = $scope.kneadingCommandSelect.Quantity;
                for (var index = 0; index < $scope.productSearchResult.products.length; index++) {
                    var product = $scope.productSearchResult.products[index];
                    remain -= product.CommandQty;
                }

                productDetails.TabletisingQuantity = remain;
                $scope.editorMode = $scope.steps.EditProductDetails;

                $scope.productDetailSearchResult['productDetails'].push(productDetails);

                var iRemaining = $scope.getRemainingQuantity();
                if (iRemaining > 0.005) {
                    $scope.go();
                }
            }

            /**
             * Check a string whether is empty or null.
             * @param {} info 
             * @returns {} 
             */
            $scope.isNullOrEmpty = function (info) {
                return info == null || info.length < 1;

            }

            /**
             * Delete product detail from list.
             * @param {} productDetail 
             * @returns {} 
             */
            $scope.deleteProductDetail = function (productDetail) {

                // Product details list is invalid.
                if ($scope.productDetailSearchResult == null || $scope.productDetailSearchResult.productDetails == null || $scope.productDetailSearchResult.productDetails.length < 1)
                    return;

                var index = $scope.productDetailSearchResult.productDetails.indexOf(productDetail);
                if (index < 0)
                    return;

                $scope.productDetailSearchResult.productDetails.splice(index, 1);
                productDetail = null;
            }

            /**
             * Initiate tabletising command
             * @returns {} 
             */
            $scope.initiateTabletisingCommand = function () {

            }

            /**
             * Get remaining of product.
             * @param {} kneadingCommandQuantity 
             * @param {} productDetails 
             * @returns {} 
             */
            $scope.getRemainingQuantity = function () {

                // No valid kneading command has been selected.
                if ($scope.kneadingCommandSelect == null)
                    return null;

                // Calculate used quantity.
                if ($scope.productSearchResult == null || $scope.productSearchResult.products == null || $scope.productSearchResult.products.length < 1)
                    return null;

                var iTotal = 0;
                var details = $scope.productDetailSearchResult.productDetails;
                for (var index = 0; index < $scope.productDetailSearchResult.productDetails.length; index++) {
                    var detail = details[index];
                    iTotal += detail.UsedPreProduct;
                }

                return $scope.kneadingCommandSelect.Quantity - iTotal;
            }

            /**
             * Select a kneading command
             * @param {} kneadingCommand 
             * @returns {} 
             */
            $scope.chooseKneadingCommand = function (kneadingCommand) {

                if ($scope.editorMode != $scope.steps.LoadKneadingCommand)
                    return;

                $scope.kneadingCommandSelect = kneadingCommand;
                $scope.$applyAsync();
            }

            /**
             * Choose a product.
             * @param {} product 
             * @returns {} 
             */
            $scope.chooseProduct = function (product) {

                if ($scope.editorMode != $scope.steps.LoadProductInformation)
                    return;

                $scope.productSelect = product;
                $scope.$applyAsync();
            }

            /**
             * Start tabletising command.
             * @returns {} 
             */
            $scope.go = function () {

                //	If absolute value of [remain_quantity] <= 0.005, the system will show message as MSG 17. User clicks on “OK” button to continue saving data as BR 9. User clicks on “Cancel” to close alert, nothing changes.
                var iRemaining = $scope.getRemainingQuantity();
                if (iRemaining == null)
                    return;
                 
                if (Math.abs(iRemaining) <= 0.005) {
                    // Display confirmation message.
                    if (!confirm("Are you sure to add or update these Tabletising Commands?"))
                        return;

                    $scope.tabletizeCommand();
                    return;
                }

                //	If [remain_quantity] > 0.005, the system will show message as MSG 18. User clicks on “OK” button to continue processing as BR 8. 
                //User clicks on “Cancel” to close alert, nothing changes.
                if (iRemaining > 0.005) {
                    //if (!confirm("The used pre-product quantity is less than total quantity, add the remainder to the last item?"))
                    //    return;

                    if ($scope.productDetailSearchResult == null ||
                        $scope.productDetailSearchResult.productDetails == null)
                        return;

                    var details = $scope.productDetailSearchResult.productDetails;
                    for (var i = 0; i < details.length; i++) {
                        var detail = $scope.productDetailSearchResult.productDetails[i];
                        var iVal = detail.UsedPreProduct + iRemaining;
                        detail.UsedPreProduct = iVal;
                        detail.TabletisingQuantity = iVal * $scope.productSelect.Yieldrate / 100;
                    }

                    $scope.$applyAsync();
                    return;
                }

                var data = { Message: "The used pre-product quantity is more than total quantity." };
                showMessage(data);
            }

            /**
             * Tabletize command.
             * @returns {} 
             */
            $scope.tabletizeCommand = function () {

                var condition = {
                    kneadingNo: $scope.kneadingCommandSelect.KneadingNo,
                    preProductCode: $scope.kneadingCommandSelect.PreproductCode,
                    lotNo: $scope.kneadingCommandSelect.LotNo,
                    remaining: $scope.getRemainingQuantity(),
                    items: $scope.productDetailSearchResult.productDetails
                }

                // Tabletize command.
                tabletisingCommandService.tabletizeCommand(condition)
                    .then(function (x) {
                        if (x == null || x['data'] == null || x['data']['result'] == null)
                            return;

                        x = x.data.result;
                        if (x == 1) {
                            var data = { Message: 'Container has been taken out, tablet command cannot change' };
                            showMessage(data);
                            return;
                        }

                        // Reload the page.
                        location.reload();
                    });
            }

            /**
             * Update tabletising command.
             * @returns {} 
             */
            $scope.updateTabletizingCommand = function () {

                var conditions = {
                    productCode: $scope.productSelect.F09_ProductCode,
                    quantity: $scope.getRemainingQuantity()
                }

                tabletisingCommandService.updateTabletizingCommand(conditions)
                    .then(
                    function () {
                        // Reload window.
                        location.reload();
                    },
                    function (response) {
                        if (response.message != null)
                            showMessage(response);
                    });
            }

            /**
             * Detail click.
             * @param {} preProductCode 
             * @param {} lotNo 
             * @returns {} 
             */
            $scope.clickDetail = function(preProductCode, lotNo) {
                $.ajax({
                    url: "/TabletisingCommandSubSystem/CreateTabletisingCommand/Details",
                    type: "GET",
                    data: {
                        preProductCode: preProductCode,
                        lotNo: lotNo
                    },
                    success: function (data) {

                        $('#productDetailModal').modal('toggle');
                        $('#productDetailModal .modal-body').html(data);
                    }
                });
            }
        }
    ]);
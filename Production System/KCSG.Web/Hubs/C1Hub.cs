﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Web.Attributes;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace KCSG.Web.Hubs
{
    [SignalrCookieAuthenticate(null)]
    [HubName("Communication-C1")]
    public class C1Hub : Hub
    {
        #region Properties

        /// <summary>
        /// Unit of work IoC.
        /// </summary>
        private IUnitOfWork _unitOfWork;

        /// <summary>
        /// Domain which is used for handling account login.
        /// </summary>
        private ILoginDomain _loginDomain;

        /// <summary>
        /// Unit of work IoC.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
        }

        /// <summary>
        /// Domain which is used for handling account login.
        /// </summary>
        public ILoginDomain LoginDomain
        {
            get { return _loginDomain; }
            set { _loginDomain = value; }
        }

        public ILog Log { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate hub with IoC.
        /// </summary>
        public C1Hub()
        {
#if !UNAUTHORIZED_DEBUG
            _unitOfWork = GlobalHost.DependencyResolver.Resolve<IUnitOfWork>();
            _loginDomain = GlobalHost.DependencyResolver.Resolve<ILoginDomain>();
            Log = LogManager.GetLogger(typeof(C1Hub));
#endif
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Callback which is fired when client connects to hub.
        /// </summary>
        /// <returns></returns>
        public override  Task OnConnected()
        {
#if !UNAUTHORIZED_DEBUG
            try
            {
                // Find httpContext of request.
                var httpContext = Context.Request.GetHttpContext();

                // Find ip address comes from request.
                var ipAddress = _loginDomain.FindRequestIpAddress(httpContext);

                // Analyze ip address.
                var ips = _loginDomain.AnalyzeIpAddress(ipAddress);

                // Find terminal from ip address.
                var terminal =  _loginDomain.FindTerminalFromIpAddressAsync(ips);

                Log.Debug(
                    string.Format(
                        "(C1) IP : {0} - Terminal : {1} - Connection Index: {2} has connected. Await for checking...",
                        ipAddress, terminal, Context.ConnectionId));

                // Update realtime connection record into database.
                var realtimeConnections = _unitOfWork.RealtimeConnectionRepository.GetAll();
                realtimeConnections = realtimeConnections.Where(x => x.Index.Equals(Context.ConnectionId));

                // Find the connection.
                var realtimeConnection =  realtimeConnections.FirstOrDefault();
                if (realtimeConnection == null)
                {
                    realtimeConnection = new RealtimeConnection();
                    Log.Debug("(C1) Initiate new connection record");
                }

                realtimeConnection.Index = Context.ConnectionId;
                realtimeConnection.Joined = DateTime.Now;
                realtimeConnection.TerminalNo = terminal.F06_TerminalNo;
                _unitOfWork.RealtimeConnectionRepository.AddOrUpdate(realtimeConnection);

                // Commit changes asynchronously.
                 _unitOfWork.Commit();
                Log.Debug(string.Format("(C1) Connection {0} of terminal {1} ", realtimeConnection.Index, terminal.F06_TerminalNo));
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, exception);
            }
#endif

          return   base.OnConnected();
        }

        /// <summary>
        /// Callback which is fired when client disconnects from hub.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override  Task OnDisconnected(bool stopCalled)
        {

#if !UNAUTHORIZED_DEBUG

            Log.Info(string.Format("Connection index: {0} has been disconnected", Context.ConnectionId));
            return Task.FromResult(0);

            Log.Debug(string.Format("(C1) Client {0}  disconnected.", Context.ConnectionId));

            // Delete the connection index from database.
            _unitOfWork.RealtimeConnectionRepository.Delete(x => x.Index.Equals(Context.ConnectionId));

            // Commit changes asynchronously.
             _unitOfWork.Commit();
#endif
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Callback which is fired when client reconnects to hub.
        /// </summary>
        /// <returns></returns>
        public override Task OnReconnected()
        {
#if !UNAUTHORIZED_DEBUG
            Log.Info(string.Format("Connection index: {0} has been re-connected", Context.ConnectionId));
            OnConnected();
#endif
            return Task.FromResult(0);
        }

        #endregion
    }
}
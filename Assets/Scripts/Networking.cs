using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting.UTP;
using TMPro;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Relay.Models;
using UnityEngine.UI;

public class Networking : MonoBehaviour
{
    
    [SerializeField] private String _joinCode;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private TextMeshProUGUI _displayJoin;
    [SerializeField] private TextMeshProUGUI _textField;

    private Boolean joined; 
    // Start is called before the first frame update
    void Start()
    {
    
        _joinCode = "";
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(joined);
    }

    public void SetJoinCode(String joinCode)
    {
        _joinCode = joinCode;
    }
    
    public async void OnClick_Server()
    {
        if (_networkManager == null)
            return;

        _joinCode = await StartHostWithRelay();
        _displayJoin.text = "Join Code: " +_joinCode;
    }
    
    
    public async void OnClick_Client()
    {
        if (_networkManager == null)
            return;
        _joinCode = _textField.text.Substring(0, 6);
        
        Boolean joined = await StartClientWithRelay(_joinCode);
        
    }

    
    public async Task<string> StartHostWithRelay(int maxConnections = 7)
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Request allocation and join code
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        // Configure transport
        var unityTransport = _networkManager.TransportManager.GetTransport<FishyUnityTransport>();
        unityTransport.SetRelayServerData(new RelayServerData(allocation, "dtls"));

        // Start host
        if (_networkManager.ServerManager.StartConnection()) // Server is successfully started.
        {
            _networkManager.ClientManager.StartConnection();
            return joinCode;
        }
        return null;
    }
    
    
        /// <summary>
    /// Joins a game with relay: it will initialize the Unity services, sign in anonymously, join the relay with the given join code and start the client.
    /// </summary>
    /// <param name="joinCode">The join code of the allocation</param>
    /// <returns>True if starting the client was successful</returns>
    /// <exception cref="ServicesInitializationException"> Exception when there's an error during services initialization </exception>
    /// <exception cref="UnityProjectNotLinkedException"> Exception when the project is not linked to a cloud project id </exception>
    /// <exception cref="CircularDependencyException"> Exception when two registered <see cref="IInitializablePackage"/> depend on the other </exception>
    /// <exception cref="AuthenticationException"> The task fails with the exception when the task cannot complete successfully due to Authentication specific errors. </exception>
    /// <exception cref="RequestFailedException">Thrown when the request does not reach the Relay Allocation service.</exception>
    /// <exception cref="ArgumentException">Thrown if the joinCode has the wrong format.</exception>
    /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation service but results in an error.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the UnityTransport component cannot be found.</exception>
    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Join allocation
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        // Configure transport
        var unityTransport = _networkManager.TransportManager.GetTransport<FishyUnityTransport>();
        unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        // Start client
        return _networkManager.ClientManager.StartConnection();
    }
    
}

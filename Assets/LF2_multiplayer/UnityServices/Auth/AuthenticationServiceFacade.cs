using System;
using System.Threading.Tasks;
using Unity.Multiplayer.Samples.BossRoom.Shared.Infrastructure;
using Unity.Multiplayer.Samples.BossRoom.Shared.Net.UnityServices.Infrastructure;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace BossRoom.Scripts.Shared.Net.UnityServices.Auth
{
    public class AuthenticationServiceFacade
    {
        IPublisher<UnityServiceErrorMessage> m_UnityServiceErrorMessagePublisher;

        [Inject]
        void InjectDependencies(IPublisher<UnityServiceErrorMessage> unityServiceErrorMessagePublisher)
        {
            m_UnityServiceErrorMessagePublisher = unityServiceErrorMessagePublisher;
        }



        public async Task InitializeUGS_Async(InitializationOptions initializationOptions)
        {
            try
            {
                // Initialize Unity Services
                await Unity.Services.Core.UnityServices.InitializeAsync(initializationOptions);

            }
            catch (Exception e)
            {
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task SignInAnonymous_Async()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // Debug.Log("SignInAnonymous_Async");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        
            
        }
        
        public async Task LinkWithGoogleAsync(string idToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);
                Debug.Log("Link is successful.");
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                // Prompt the player with an error message.
                Debug.LogError("This user is already linked with another account. Log in instead.");
            }

            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }


        public async Task SwitchProfileAndReSignInAsync(string profile)
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut();
            }
            AuthenticationService.Instance.SwitchProfile(profile);


            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

        public async Task<bool> EnsurePlayerIsAuthorized()
        {
            if (AuthenticationService.Instance.IsAuthorized)
            {
                return true;
            }

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                return true;
            }
            catch (AuthenticationException e)
            {
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                //not rethrowing for authentication exceptions - any failure to authenticate is considered "handled failure"
                return false;
            }
            catch (Exception e)
            {
                //all other exceptions should still bubble up as unhandled ones
                var reason = $"{e.Message} ({e.InnerException?.Message})";
                m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
                throw;
            }
        }

    }
}

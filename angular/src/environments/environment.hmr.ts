export const environment = {
  production: false,
  hmr: true,
  application: {
    name: 'SettingUi',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44318',
    clientId: 'SettingUi_ConsoleTestApp',
    dummyClientSecret: '1q2w3e*',
    scope: 'SettingUi',
    showDebugInformation: true,
    oidc: false,
    requireHttps: true,
  },
  apis: {
    default: {
      url: 'https://localhost:44365',
    },
  },
  localization: {
    defaultResourceName: 'SettingUi',
  },
};

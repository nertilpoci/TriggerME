import { TriggerMe.WebClientPage } from './app.po';

describe('trigger-me.web-client App', () => {
  let page: TriggerMe.WebClientPage;

  beforeEach(() => {
    page = new TriggerMe.WebClientPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});

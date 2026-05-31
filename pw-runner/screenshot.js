const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({
    executablePath: 'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe',
    headless: true,
    args: ['--no-sandbox', '--disable-setuid-sandbox']
  });

  const page = await browser.newPage();
  await page.setViewportSize({ width: 1440, height: 900 });

  // 1. Login page
  await page.goto('http://localhost:5050/Admin/Account/Login');
  await page.waitForLoadState('networkidle');
  await page.screenshot({ path: '../screenshot_login.png' });
  console.log('1. Login page captured');

  // 2. Log in with seeded admin credentials
  await page.fill('input[name="Email"]', 'admin@samn.org');
  await page.fill('input[name="Password"]', 'Admin123!');
  await page.click('button[type="submit"]');
  await page.waitForLoadState('networkidle');
  console.log('After login URL:', page.url());

  if (page.url().includes('Login')) {
    // try superadmin
    await page.fill('input[name="Email"]', 'superadmin@samn.org');
    await page.fill('input[name="Password"]', 'SuperAdmin2024!@#');
    await page.click('button[type="submit"]');
    await page.waitForLoadState('networkidle');
    console.log('After superadmin login URL:', page.url());
  }

  // 3. Dashboard
  await page.screenshot({ path: '../screenshot_dashboard.png', fullPage: true });
  console.log('2. Dashboard captured');

  // 4. Settings page
  await page.goto('http://localhost:5050/Admin/System/Settings');
  await page.waitForLoadState('networkidle');
  await page.screenshot({ path: '../screenshot_settings.png', fullPage: true });
  console.log('3. Settings page captured');

  // 5. Settings - Email tab
  await page.click('.settings-tab:nth-child(2)');
  await page.waitForTimeout(400);
  await page.screenshot({ path: '../screenshot_settings_email.png', fullPage: true });
  console.log('4. Settings email tab captured');

  // 6. Settings - Actions tab
  await page.click('.settings-tab:nth-child(3)');
  await page.waitForTimeout(400);
  await page.screenshot({ path: '../screenshot_settings_actions.png', fullPage: true });
  console.log('5. Settings actions tab captured');

  // 7. Members page
  await page.goto('http://localhost:5050/Admin/Members/Index');
  await page.waitForLoadState('networkidle');
  await page.screenshot({ path: '../screenshot_members.png', fullPage: true });
  console.log('6. Members page captured');

  await browser.close();
  console.log('Done!');
})().catch(e => { console.error('ERROR:', e.message); process.exit(1); });

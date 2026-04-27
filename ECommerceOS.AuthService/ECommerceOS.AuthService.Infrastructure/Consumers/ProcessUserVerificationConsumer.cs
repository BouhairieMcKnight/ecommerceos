using ECommerceOS.AuthService.Infrastructure.Saga;
using MassTransit;
using Microsoft.AspNetCore.Identity.UI.Services;
using ZiggyCreatures.Caching.Fusion;

namespace ECommerceOS.AuthService.Infrastructure.Consumers;

public class ProcessUserVerificationConsumer(
    ILogger<ProcessUserVerificationConsumer> logger,
    IEmailSender emailSender,
    IFusionCache cache,
    IEncryptionService encryptionService,
    Random random) 
    : IConsumer<VerifyUser>
{
    private const string VerifySubject = "ECommerceOS Verification Code";
    
    public async Task Consume(ConsumeContext<VerifyUser> context)
    {
        logger.LogInformation("Sending verification email");
        Console.WriteLine("Sending verification email");
        
        var code = random.Next(0, 99999).ToString("D5");
        
        var key = $"{context.Message.UserId.Value:N} {context.Message.EmailAddress}";
        var parameter = Uri.EscapeDataString(encryptionService.Encrypt(key));
        await cache.SetAsync(key, code, options =>
        {
	        options.DistributedCacheDuration = TimeSpan.FromMinutes(30);
	        options.JitterMaxDuration = TimeSpan.FromMinutes(1);
        });
        
        var message = $$"""
                        <!DOCTYPE html>
                        <html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office" lang="en">

                        <head>
                        	<title></title>
                        	<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
                        	<meta name="viewport" content="width=device-width, initial-scale=1.0"><!--[if mso]>
                        <xml><w:WordDocument xmlns:w="urn:schemas-microsoft-com:office:word"><w:DontUseAdvancedTypographyReadingMail/></w:WordDocument>
                        <o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml>
                        <![endif]--><!--[if !mso]><!-->
                        	<link href="https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&amp;display=swap" rel="stylesheet" type="text/css"><!--<![endif]-->
                        	<style>
                        		* {
                        			box-sizing: border-box;
                        		}

                        		body {
                        			margin: 0;
                        			padding: 0;
                        		}

                        		a[x-apple-data-detectors] {
                        			color: inherit !important;
                        			text-decoration: inherit !important;
                        		}

                        		#MessageViewBody a {
                        			color: inherit;
                        			text-decoration: none;
                        		}

                        		p {
                        			line-height: inherit
                        		}

                        		.desktop_hide,
                        		.desktop_hide table {
                        			mso-hide: all;
                        			display: none;
                        			max-height: 0px;
                        			overflow: hidden;
                        		}

                        		.image_block img+div {
                        			display: none;
                        		}

                        		sup,
                        		sub {
                        			font-size: 75%;
                        			line-height: 0;
                        		}

                        		@media (max-width:720px) {

                        			.desktop_hide table.icons-inner,
                        			.row-4 .column-2 .block-2.button_block .alignment .button {
                        				display: inline-block !important;
                        			}

                        			.icons-inner {
                        				text-align: center;
                        			}

                        			.icons-inner td {
                        				margin: 0 auto;
                        			}

                        			.mobile_hide {
                        				display: none;
                        			}

                        			.row-content {
                        				width: 100% !important;
                        			}

                        			.stack .column {
                        				width: 100%;
                        				display: block;
                        			}

                        			.mobile_hide {
                        				min-height: 0;
                        				max-height: 0;
                        				max-width: 0;
                        				overflow: hidden;
                        				font-size: 0px;
                        			}

                        			.desktop_hide,
                        			.desktop_hide table {
                        				display: table !important;
                        				max-height: none !important;
                        			}

                        			.row-3 .column-1 .block-2.heading_block td.pad {
                        				padding: 40px 10px !important;
                        			}

                        			.row-3 .column-1 .block-2.heading_block h1 {
                        				font-size: 32px !important;
                        			}

                        			.row-4 .column-2 .block-1.paragraph_block td.pad>div {
                        				text-align: left !important;
                        				font-size: 16px !important;
                        			}

                        			.row-4 .column-2 .block-1.paragraph_block td.pad {
                        				padding: 8px 2px 5px !important;
                        			}

                        			.row-1 .column-1 .block-1.paragraph_block td.pad>div {
                        				text-align: center !important;
                        				font-size: 13px !important;
                        			}

                        			.row-1 .column-1 .block-1.paragraph_block td.pad {
                        				padding: 10px !important;
                        			}

                        			.row-5 .column-1 .block-1.spacer_block,
                        			.row-6 .column-1 .block-1.spacer_block {
                        				height: 24px !important;
                        			}

                        			.row-4 .column-2 .block-2.button_block td.pad {
                        				padding: 8px 0 0 !important;
                        			}

                        			.row-4 .column-2 .block-2.button_block span {
                        				font-size: 16px !important;
                        				line-height: 32px !important;
                        			}

                        			.row-4 .column-2 .block-2.button_block .alignment {
                        				text-align: left !important;
                        			}

                        			.row-7 .column-1 .block-1.paragraph_block td.pad>div {
                        				text-align: center !important;
                        				font-size: 10px !important;
                        			}

                        			.row-7 .column-1 .block-1.paragraph_block td.pad {
                        				padding: 24px !important;
                        			}

                        			.row-3 .row-content,
                        			.row-4 .column-1,
                        			.row-7 .column-1 {
                        				padding: 0 !important;
                        			}

                        			.row-2 .column-1 .col-pad {
                        				padding: 16px 0 24px !important;
                        			}

                        			.row-4 .column-2 .col-pad {
                        				padding: 24px 24px 0 !important;
                        			}
                        		}
                        	</style><!--[if mso ]><style>sup, sub { font-size: 100% !important; } sup { mso-text-raise:10% } sub { mso-text-raise:-10% }</style> <![endif]-->
                        </head>

                        <body class="body" style="background-color: #ffffff; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;">
                        	<table class="nl-container" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;">
                        		<tbody>
                        			<tr>
                        				<td>
                        					<table class="row row-1" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-bottom: 8px solid #ffffff; border-left: 8px solid #ffffff; border-radius: 0; border-right: 8px solid #ffffff; border-top: 8px solid #ffffff; color: #000000; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="100%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; background-color: #f9f9fb; vertical-align: top; border-radius: 8px;">
                        													<table class="paragraph_block block-1" width="100%" border="0" cellpadding="10" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;">
                        														<tr>
                        															<td class="pad">
                        																<div style="color:#040b22;direction:ltr;font-family:'Inter','Helvetica';font-size:14px;font-weight:400;letter-spacing:0px;line-height:1.2;text-align:center;mso-line-height-alt:17px;">
                        																	<p style="margin: 0;">View this email in your browser</p>
                        																</div>
                        															</td>
                        														</tr>
                        													</table>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table>
                        					<table class="row row-2" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-radius: 0; color: #000000; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="100%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top;">
                        													<table width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        														<tr>
                        															<td class="col-pad" style="padding-bottom:32px;padding-top:24px;">
                        																<table class="image_block block-1" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        																	<tr>
                        																		<td class="pad" style="width:100%;padding-right:0px;padding-left:0px;">
                        																			<div class="alignment" align="center">
                        																				<div style="max-width: 70px;"><a href="https://www.example.com/" target="_blank"><img src="https://d1oco4z2z1fhwp.cloudfront.net/templates/default/10641/Klera.png" style="display: block; height: auto; border: 0; width: 100%;" width="70" alt="Logo" title="Logo" height="auto"></a></div>
                        																			</div>
                        																		</td>
                        																	</tr>
                        																</table>
                        															</td>
                        														</tr>
                        													</table>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table>
                        					<table class="row row-3" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-position: top center; background-repeat: no-repeat; color: #000000; background-image: url('https://d1oco4z2z1fhwp.cloudfront.net/templates/default/10641/OptIn_Hero.png'); border-radius: 0; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="100%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top;">
                        													<div class="spacer_block block-1" style="height:23px;line-height:23px;font-size:1px;">&#8202;</div>
                        													<table class="heading_block block-2" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        														<tr>
                        															<td class="pad" style="padding-bottom:60px;padding-left:10px;padding-right:10px;padding-top:60px;text-align:center;width:100%;">
                        																<h1 style="margin: 0; color: #ffffff; direction: ltr; font-family: 'Inter','Helvetica'; font-size: 32px; font-weight: 600; letter-spacing: -1px; line-height: 1.2; text-align: center; margin-top: 0; margin-bottom: 0; mso-line-height-alt: 38px;"><span class="tinyMce-placeholder" style="word-break: break-word;">Confirm your<br>subscription</span></h1>
                        															</td>
                        														</tr>
                        													</table>
                        													<div class="spacer_block block-3" style="height:23px;line-height:23px;font-size:1px;">&#8202;</div>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table>
                        					<table class="row row-4" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-position: top center; background-repeat: no-repeat; color: #000000; background-color: #ffffff; background-image: url('https://d1oco4z2z1fhwp.cloudfront.net/templates/default/10641/Grey_BG_01.png'); border-radius: 0; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="16.666666666666668%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-radius: 0px;">
                        													<div class="spacer_block block-1 mobile_hide" style="height:60px;line-height:60px;font-size:1px;">&#8202;</div>
                        												</td>
                        												<td class="column column-2" width="66.66666666666667%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-radius: 0px;">
                        													<table width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        														<tr>
                        															<td class="col-pad" style="padding-bottom:30px;padding-left:8px;padding-right:8px;padding-top:40px;">
                        																<table class="paragraph_block block-1" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;">
                        																	<tr>
                        																		<td class="pad" style="padding-bottom:12px;padding-left:10px;padding-right:10px;">
                        																			<div style="color:#4a4f5f;direction:ltr;font-family:'Inter','Helvetica';font-size:16px;font-weight:300;letter-spacing:0px;line-height:1.2;text-align:left;mso-line-height-alt:19px;">
                        																				<p style="margin: 0; margin-bottom: 8px;">Thanks for signing up! Before we start sending you product updates, tips, and resources, we just need to confirm it’s really you.</p>
                        																				<p style="margin: 0;">If you didn’t request this, feel free to ignore this email.</p>
                        																				<p style="margin: 0;">Your Confirmation Code is <b>{{code}}</b></p>
                        																			</div>
                        																		</td>
                        																	</tr>
                        																</table>
                        																<table class="button_block block-2" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        																	<tr>
                        																		<td class="pad" style="padding-bottom:10px;padding-left:8px;padding-right:10px;padding-top:8px;text-align:center;">
                        																			<div class="alignment" align="center"><a href="https://localhost:8001/api/auth/verify?url={{parameter}}" target="_blank" style="color:#ffffff;text-decoration:none;"><!--[if mso]>
                        <v:roundrect xmlns:v="urn:schemas-microsoft-com:vml" xmlns:w="urn:schemas-microsoft-com:office:word"  href="https://www.example.com/"  style="height:42px;width:245px;v-text-anchor:middle;" arcsize="77%" fillcolor="#1e40ff">
                        <v:stroke dashstyle="Solid" weight="0px" color="#1e40ff"/>
                        <w:anchorlock/>
                        <v:textbox inset="0px,0px,0px,0px">
                        <center dir="false" style="color:#ffffff;font-family:sans-serif;font-size:16px">
                        <![endif]--><span class="button" style="background-color: #1e40ff; border-bottom: 0px solid transparent; border-left: 0px solid transparent; border-radius: 32px; border-right: 0px solid transparent; border-top: 0px solid transparent; color: #ffffff; display: inline-block; font-family: 'Inter','Helvetica'; font-size: 16px; font-weight: 400; mso-border-alt: none; text-align: center; width: auto; word-break: keep-all; letter-spacing: normal;"><span class="btn-pad" style="word-break: break-word; padding-left: 24px; padding-right: 24px; padding-top: 5px; padding-bottom: 5px; display: block;"><span style="word-break: break-word; line-height: 32px;">Confirm my email address</span></span></span><!--[if mso]></center></v:textbox></v:roundrect><![endif]--></a></div>
                        																		</td>
                        																	</tr>
                        																</table>
                        															</td>
                        														</tr>
                        													</table>
                        												</td>
                        												<td class="column column-3" width="16.666666666666668%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; border-radius: 0px;">
                        													<table width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        														<tr>
                        															<td class="col-pad" style="padding-bottom:5px;padding-top:5px;">
                        																<div class="spacer_block block-1 mobile_hide" style="height:60px;line-height:60px;font-size:1px;">&#8202;</div>
                        															</td>
                        														</tr>
                        													</table>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table><!--[if !mso]><!-->
                        					<table class="row row-5 desktop_hide" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; mso-hide: all; display: none; max-height: 0; overflow: hidden;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; mso-hide: all; display: none; max-height: 0; overflow: hidden; background-color: #f5f5f7; border-radius: 0; color: #000000; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="100%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top;">
                        													<div class="spacer_block block-1" style="height:80px;line-height:80px;font-size:1px;">&#8202;</div>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table><!--<![endif]-->
                        					<table class="row row-6" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; border-radius: 0; color: #000000; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="100%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top;">
                        													<div class="spacer_block block-1" style="height:80px;line-height:80px;font-size:1px;">&#8202;</div>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table>
                        					<table class="row row-7" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f5f5f7; border-radius: 24px 24px 0 0; color: #000000; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="100%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top;">
                        													<table class="paragraph_block block-1" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;">
                        														<tr>
                        															<td class="pad" style="padding-bottom:44px;padding-left:10px;padding-right:10px;padding-top:44px;">
                        																<div style="color:#4a4f5f;direction:ltr;font-family:'Inter','Helvetica';font-size:14px;font-weight:400;letter-spacing:0px;line-height:1.6;text-align:center;mso-line-height-alt:22px;">
                        																	<p style="margin: 0; margin-bottom: 0px;">You received this email because you created an account or subscribed to updates from Klera.<br><br>&nbsp;ECommerceOS Inc.</p>
                        																	<p style="margin: 0; margin-bottom: 0px;">2135 Alder Street, Suite 402</p>
                        																	<p style="margin: 0; margin-bottom: 0px;">Vancouver, BC V6J 3R4, Canada<br><br>To update your communication settings or to unsubscribe, use the links below.<br><strong><a href="https://www.example.com/" target="_blank" style="text-decoration: none; color: #4a4f5f;" rel="noopener">Manage Preferences</a> | <a href="https://www.example.com/" target="_blank" style="text-decoration: none; color: #4a4f5f;" rel="noopener">Unsubscribe</a></strong></p>
                        																	<p style="margin: 0;">© 2025 Klera Technologies Inc. All rights reserved.</p>
                        																</div>
                        															</td>
                        														</tr>
                        													</table>
                        													<table class="image_block block-2" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        														<tr>
                        															<td class="pad" style="width:100%;">
                        																<div class="alignment" align="center">
                        																	<div style="max-width: 671px;"><a href="https://www.example.com/" target="_blank"><img src="https://d1oco4z2z1fhwp.cloudfront.net/templates/default/10641/Logo_Large.png" style="display: block; height: auto; border: 0; width: 100%;" width="671" alt="Logo" title="Logo" height="auto"></a></div>
                        																</div>
                        															</td>
                        														</tr>
                        													</table>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table>
                        					<table class="row row-8" align="center" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;">
                        						<tbody>
                        							<tr>
                        								<td>
                        									<table class="row-content stack" align="center" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff; color: #000000; width: 700px; margin: 0 auto;" width="700">
                        										<tbody>
                        											<tr>
                        												<td class="column column-1" width="100%" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top;">
                        													<table width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt;">
                        														<tr>
                        															<td class="col-pad" style="padding-bottom:5px;padding-top:5px;">
                        																<table class="icons_block block-1" width="100%" border="0" cellpadding="0" cellspacing="0" role="presentation" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; text-align: center; line-height: 0;">
                        																	<tr>
                        																		<td class="pad" style="vertical-align: middle; color: #1e0e4b; font-family: 'Inter', sans-serif; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;"><!--[if vml]><table align="center" cellpadding="0" cellspacing="0" role="presentation" style="display:inline-block;padding-left:0px;padding-right:0px;mso-table-lspace: 0pt;mso-table-rspace: 0pt;" ><![endif]-->
                        																			<!--[if !vml]><!-->
                        																			<table class="icons-inner" style="mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; padding-left: 0px; padding-right: 0px;" cellpadding="0" cellspacing="0" role="presentation"><!--<![endif]-->
                        																				<tr>
                        																					<td style="vertical-align: middle; text-align: center; padding-top: 5px; padding-bottom: 5px; padding-left: 5px; padding-right: 6px;"><a href="https://designedwithbeefree.com/" target="_blank" title="Designed with Beefree" style="text-decoration: none;"><img class="icon" alt="Beefree Logo" src="https://d1oco4z2z1fhwp.cloudfront.net/assets/Beefree-logo.png" height="auto" width="34" align="center" style="display: block; height: auto; margin: 0 auto; border: 0;"></a></td>
                        																					<td style="font-family: 'Inter', sans-serif; font-size: 15px; font-weight: undefined; color: #1e0e4b; vertical-align: middle; letter-spacing: normal; text-align: center; line-height: normal;"><a href="https://designedwithbeefree.com/" target="_blank" title="Designed with Beefree" style="color: #1e0e4b; text-decoration: none;">Designed with Beefree</a></td>
                        																				</tr>
                        																			</table>
                        																		</td>
                        																	</tr>
                        																</table>
                        															</td>
                        														</tr>
                        													</table>
                        												</td>
                        											</tr>
                        										</tbody>
                        									</table>
                        								</td>
                        							</tr>
                        						</tbody>
                        					</table>
                        				</td>
                        			</tr>
                        		</tbody>
                        	</table><!-- End -->
                        </body>

                        </html>
                        """;
        
        await emailSender.SendEmailAsync(context.Message.EmailAddress, VerifySubject, message);
    }
}
<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:key name="SourceItemKey" match="Event" use="@SourceItem"/>
  
  <xsl:variable name="ProjectEvents">
    <ProjectEvents>
      <xsl:attribute name="Project">
        <xsl:value-of select="/UpgradeLog/Properties/Property[@Name='Project']/@Value"/>
      </xsl:attribute>      
        <xsl:for-each select="/UpgradeLog/Events/Event[count(. | key('SourceItemKey',@SourceItem)[1]) = 1]">   
        <xsl:sort select="@SourceItem" order="ascending"/>     
        <xsl:if test="(1=position()) or (preceding-sibling::*[1]/@SourceItem != @SourceItem)">
          <GroupedProjectEvents>            
            <xsl:attribute name="SourceItem">
              <xsl:value-of select="@SourceItem"/>
            </xsl:attribute>

            <xsl:variable name="SourceItem">
              <xsl:value-of select="@SourceItem"/>
            </xsl:variable>
            
             <xsl:attribute name="Action">
              <xsl:value-of select="@Action"/>
            </xsl:attribute>

            <xsl:variable name="Action">
              <xsl:value-of select="@Action"/>
            </xsl:variable>

            <xsl:for-each select="key('SourceItemKey', $SourceItem)[ @SourceItem = $SourceItem ]">
              <Event>
                <xsl:attribute name="Message">
                  <xsl:value-of select="@Message"/>
                </xsl:attribute>
                <xsl:attribute name="SourceItem">
                  <xsl:value-of select="@SourceItem"/>
                </xsl:attribute>
                <xsl:attribute name="EventType">
                  <xsl:value-of select="@EventType"/>
                </xsl:attribute>
                <xsl:attribute name="Action">
                  <xsl:value-of select="@Action"/>
                </xsl:attribute>
              </Event>
            </xsl:for-each>
          </GroupedProjectEvents>
        </xsl:if>
      </xsl:for-each>
    </ProjectEvents>
  </xsl:variable>
  
  <xsl:template match="ProjectEvents">    
    <table cellpadding="2" cellspacing="0" width="100%" border="1" bordercolor="white" class="infotable">
      <tr>
        <td nowrap="1" class="header">File</td>
        <td nowrap="1" class="header" width="13"/>
        <td nowrap="1" class="header">Status</td>     
        <td nowrap="1" class="header">Errors</td>
        <td nowrap="1" class="header">Warnings</td>
        <td nowrap="1" class="header">Upgraded</td>
        <td nowrap="1" class="header">Analyzed</td>
      </tr>
      <xsl:for-each select="GroupedProjectEvents">      
        <xsl:sort select="count(Event[@Action='Upgrade'])" order="descending"/> 
        <xsl:variable name="source-id" select="generate-id(.)"/>
        <xsl:variable name="eventsCount" select="count(Event)"/>
        <xsl:variable name="successesCount" select="count(Event[@EventType='Success'])"/>
        <xsl:variable name="errorsCount" select="count(Event[@EventType = 'Error'])"/>
        <xsl:variable name="warningsCount" select="count(Event[@EventType = 'Warning'])"/>
        <xsl:variable name="upgradesCount" select="count(Event[@Action='Upgrade'])"/>
        <tr class="row">
          <td class="content"><A HREF="javascript:"><xsl:attribute name="onClick">javascript:document.images['<xsl:value-of select="$source-id"/>'].click()</xsl:attribute><IMG border="0" _locID="IMG.alt" _locAttrData="alt"  alt="expand/collapse section" class="expandable" height="11" onclick="changepic()" src="_ConvertModelWizard_Files/ConversionWizard_Plus.gif" width="9" ><xsl:attribute name="name"><xsl:value-of select="$source-id"/></xsl:attribute><xsl:attribute name="child">src<xsl:value-of select="$source-id"/></xsl:attribute></IMG></A>&#32;<xsl:value-of select="@SourceItem"/>
          </td>
          <td class="content" width="13">
            <xsl:if test="$successesCount = $eventsCount"><IMG src="_ConvertModelWizard_Files/Success.png" width="13" height="13"></IMG></xsl:if>
            <xsl:if test="$errorsCount > 0"><IMG src="_ConvertModelWizard_Files/Error.png" width="13" height="13"></IMG></xsl:if>
            <xsl:if test="$errorsCount = 0 and $warningsCount > 0"><IMG src="_ConvertModelWizard_Files/Warning.png" width="13" height="13"></IMG></xsl:if>            
          </td>
          <td class="content">   
            <xsl:if test="$successesCount = $eventsCount">
              Success
            </xsl:if>
            <xsl:if test="$errorsCount > 0">
              Errors
            </xsl:if>
            <xsl:if test="$errorsCount = 0 and $warningsCount > 0">
             Warnings
            </xsl:if>
          </td>
          <td class="content"><xsl:value-of select="count(Event[@EventType = 'Error'])"/></td>
          <td class="content"><xsl:value-of select="count(Event[@EventType = 'Warning'])"/></td>
          <td class="content"><xsl:value-of select="count(Event[@Action='Upgrade'])"/></td>
          <td class="content"><xsl:value-of select="count(Event[@Action='Analyze'])"/></td>          
        </tr>        
        <tr class="collapsed" bgcolor="#ffffff">
            <xsl:attribute name="id">src<xsl:value-of select="$source-id"/></xsl:attribute>
            <td colspan="7">          
              <table width="100%" border="1" bordercolor="#dcdcdc" rules="cols" class="issuetable">
                <tr>
                  <td class="header" colspan="5">Name
                  </td>
                  <td class="header">Action Taken
                  </td>                
                </tr>
                  <xsl:for-each select="Event">
                     <xsl:sort select="@Action" order="ascending"/>
                    <tr>
                      <td class="issuenone" style="border-bottom:solid 1 lightgray" width="900px" colspan="5">
                        <xsl:if test="@EventType = 'Success'"><IMG src="_ConvertModelWizard_Files/Success.png" width="13" height="13"></IMG></xsl:if>
                        <xsl:if test="@EventType = 'Error'"><IMG src="_ConvertModelWizard_Files/Error.png" width="13" height="13"></IMG></xsl:if>
                        <xsl:if test="@EventType = 'Warning'"><IMG src="_ConvertModelWizard_Files/Warning.png" width="13" height="13"></IMG></xsl:if>
                        <xsl:if test="@EventType = 'Unknown'"><IMG src="_ConvertModelWizard_Files/Unknown.png" width="13" height="13"></IMG></xsl:if>
                        &#32;&#32;<xsl:value-of select="@Message"/> 
                      </td>
                      <td class="issuenone" style="border-bottom:solid 1 lightgray">                                         
                        &#32;&#32;<xsl:value-of select="@Action"/>                  
                      </td>                   
                    </tr>
                  </xsl:for-each>               
              </table>
            </td>
          </tr>
        
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template match="UpgradeLog">
    <html>
      <head>
        <META HTTP-EQUIV="Content-Type" content="text/html; charset=utf-8" />
        <link rel="stylesheet" href="_ConvertModelWizard_Files\UpgradeReferences.css" />
        <title>
          Upgrade references
        </title>
        <script language="javascript">
          function outliner () {
          oMe = window.event.srcElement
          //get child element
          var child = document.all[event.srcElement.getAttribute("child",false)];
          //if child element exists, expand or collapse it.
          if (null != child)
          child.className = child.className == "collapsed" ? "expanded" : "collapsed";
          }

          function changepic() {
          uMe = window.event.srcElement;
          var check = uMe.src.toLowerCase();
          if (check.lastIndexOf("conversionwizard_plus.gif") != -1)
          {
          uMe.src = "_ConvertModelWizard_Files/ConversionWizard_Minus.gif"
          }
          else
          {
          uMe.src = "_ConvertModelWizard_Files/ConversionWizard_Plus.gif"
          }
          }
        </script>
      </head>
      <body topmargin="0" leftmargin="0" rightmargin="0" onclick="outliner();">
        <h1>
          Upgrade References Report
        </h1>
        <p>
          <table class="note">
            <tr>
              <td nowrap="1">
              </td>
            </tr>
            <xsl:apply-templates select="Properties"/>
          </table>
        </p>        
        <xsl:apply-templates select="msxsl:node-set($ProjectEvents)/*"/>                
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>